using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using RealEstateApi.Exceptions;
using RealEstateApi.Hubs;
using RealEstateApi.Interfaces;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class PropertyListingService : IPropertyListingService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly IRepository<Guid, PropertyListing> _propertyListingRepository;
        private readonly IRepository<Guid, PropertyImage> _propertyImageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropertyListingService(IHubContext<NotificationHub> hubContext,IRepository<Guid, PropertyListing> propertyListingRepository, IRepository<Guid, PropertyImage> propertyImageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _propertyListingRepository = propertyListingRepository;
            _propertyImageRepository = propertyImageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PropertyListing> GetListingByIdAsync(Guid id)
        {
            var listing = await _propertyListingRepository.GetByIdAsync(id);
            return listing;
        }

        public async Task<IEnumerable<PropertyListing>> GetAllListings()
        {
            var listings = await _propertyListingRepository.GetAllAsync();
            return listings;
        }

        public async Task<PropertyListing> AddListingAsync(CreatePropertyListingDto listingDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            if (!userId.HasValue)
                throw new UnauthorizedAccessAppException("Unauthorized access.");

            Guid agentId;

            if (userRole == "Admin")
            {
                if (!listingDto.AgentId.HasValue)
                    throw new ArgumentRequiredException("AgentId is required when Admin is creating a listing.");

                agentId = listingDto.AgentId.Value;
            }
            else if (userRole == "Agent")
            {
                agentId = userId.Value;
            }
            else
            {
                throw new UnauthorizedAccessAppException("Only respective agents or admins can create listings.");
            }

            // check for duplicates
            var existingListings = await _propertyListingRepository.GetAllAsync();
            var duplicate = existingListings.Any(l =>
                !l.IsDeleted &&
                l.Title.Equals(listingDto.Title, StringComparison.OrdinalIgnoreCase) &&
                l.Location.Equals(listingDto.Location, StringComparison.OrdinalIgnoreCase) &&
                l.AgentId == agentId &&
                l.Price == listingDto.Price &&
                l.Bedrooms == listingDto.Bedrooms &&
                l.Bathrooms == listingDto.Bathrooms
            );

            if (duplicate)
            {
                throw new FailedOperationException("Duplicate listing detected. A similar property already exists.");
            }


            var listing = new PropertyListing
            {
                Title = listingDto.Title,
                Description = listingDto.Description,
                Price = listingDto.Price,
                Location = listingDto.Location,
                Bedrooms = listingDto.Bedrooms,
                Bathrooms = listingDto.Bathrooms,
                SquareFeet = listingDto.SquareFeet,
                AgentId = agentId
            };



            listing = await _propertyListingRepository.AddAsync(listing);

            // await _hubContext.Clients.Group("buyers").SendAsync("ReceiveListingNotification", new {
            //     Title = "New Property Listed!",
            //     ListingId = listing.Id,
            //     Location = listing.Location,
            //     Price = listing.Price
            // });

            await _hubContext.Clients.All.SendAsync("ReceiveListingNotification", new
            {
                Title = listing.Title,
                ListingId = listing.Id,
                Location = listing.Location,
                Price = listing.Price
            });


            return listing ?? throw new FailedOperationException("Unable to add listing at the moment");

        }

        public async Task<PropertyListing> UpdateListingAsync(Guid id, UpdatePropertyListingDto listingDto)
        {
            
            var existingListing = await _propertyListingRepository.GetByIdAsync(id);
            if (existingListing == null || existingListing.IsDeleted)
                throw new NotFoundException("Listing not found or has been deleted.");

            existingListing.Title = listingDto.Title ?? existingListing.Title;
            existingListing.Description = listingDto.Description ?? existingListing.Description;
            existingListing.Price = listingDto.Price ?? existingListing.Price;
            existingListing.Location = listingDto.Location ?? existingListing.Location;
            existingListing.Bedrooms = listingDto.Bedrooms ?? existingListing.Bedrooms;
            existingListing.Bathrooms = listingDto.Bathrooms ?? existingListing.Bathrooms;
            existingListing.SquareFeet = listingDto.SquareFeet ?? existingListing.SquareFeet;

            var updatedListing = await _propertyListingRepository.UpdateAsync(id, existingListing);
            return updatedListing ?? throw new FailedOperationException("Unable to update listing at the moment.");
        }


        public async Task<PropertyListing> DeleteListingAsync(Guid id)
        {
            var listing = await _propertyListingRepository.GetByIdAsync(id);
            if (listing == null || listing.IsDeleted)
            throw new NotFoundException("Listing not found or already deleted.");

            // Mark the listing as deleted
            listing.IsDeleted = true;
            await _propertyListingRepository.UpdateAsync(id, listing);

            // Soft delete associated images
            var images = await _propertyImageRepository.GetAllAsync();
            var associatedImages = images.Where(img => img.PropertyListingId == id && !img.IsDeleted);

            foreach (var image in associatedImages)
            {
                image.IsDeleted = true;
                image.DeletedAt = DateTime.UtcNow;
                await _propertyImageRepository.UpdateAsync(image.Id, image);
            }

            return listing;
        }
        
        public async Task<PagedResult<PropertyListing>> GetFilteredListingsAsync(PropertyListingQueryParametersDto query)
        {
            var listings = await _propertyListingRepository.GetAllAsync();

            // Search
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                listings = listings.Where(l =>
                    l.Title.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase) ||
                    l.Description.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase));
            }

            // Filter
            if (!string.IsNullOrWhiteSpace(query.Location))
                listings = listings.Where(l => l.Location.Contains(query.Location, StringComparison.OrdinalIgnoreCase));

            if (query.MinPrice.HasValue)
                listings = listings.Where(l => l.Price >= query.MinPrice.Value);

            if (query.MaxPrice.HasValue)
                listings = listings.Where(l => l.Price <= query.MaxPrice.Value);

            if (query.MinBedrooms.HasValue)
                listings = listings.Where(l => l.Bedrooms >= query.MinBedrooms.Value);

            if (query.MinBathrooms.HasValue)
                listings = listings.Where(l => l.Bathrooms >= query.MinBathrooms.Value);

            // Sort
            listings = query.SortBy?.ToLower() switch
            {
                "price" => query.IsDescending ? listings.OrderByDescending(l => l.Price) : listings.OrderBy(l => l.Price),
                "bedrooms" => query.IsDescending ? listings.OrderByDescending(l => l.Bedrooms) : listings.OrderBy(l => l.Bedrooms),
                "bathrooms" => query.IsDescending ? listings.OrderByDescending(l => l.Bathrooms) : listings.OrderBy(l => l.Bathrooms),
                _ => listings.OrderBy(l => l.Price)
            };

            // count for pagination
            int totalCount = listings.Count();

            // Pagination
            listings = listings
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            return new PagedResult<PropertyListing>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = listings
            };
        }

    }
}