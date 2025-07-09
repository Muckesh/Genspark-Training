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
        private readonly IPropertyImageService _propertyImageService;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly IRepository<Guid, PropertyListing> _propertyListingRepository;
        private readonly IRepository<Guid, PropertyImage> _propertyImageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PropertyListingService(IHubContext<NotificationHub> hubContext,IPropertyImageService propertyImageService,IRepository<Guid, PropertyListing> propertyListingRepository, IRepository<Guid, PropertyImage> propertyImageRepository, IHttpContextAccessor httpContextAccessor)
        {
            _hubContext = hubContext;
            _propertyImageService = propertyImageService;
            _propertyListingRepository = propertyListingRepository;
            _propertyImageRepository = propertyImageRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PropertyListingResponseDto> GetListingByIdAsync(Guid id)
        {
            var listing = await _propertyListingRepository.GetByIdAsync(id);
            var imageUrls =( await _propertyImageService.GetImageUrlsByListingIdAsync(id)).ToList();
            var listingDto = new PropertyListingResponseDto
            {
                Id = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                Price = listing.Price,
                Location = listing.Location,
                Bedrooms = listing.Bedrooms,
                Bathrooms = listing.Bathrooms,
                SquareFeet = listing.SquareFeet,
                AgentId = listing.AgentId,
                ImageUrls = imageUrls,
                CreatedAt = listing.CreatedAt,
                IsDeleted = listing.IsDeleted
            };
            return listingDto;
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
                AgentId = agentId,
                PropertyType = listingDto.PropertyType,
                ListingType = listingDto.ListingType,
                IsPetsAllowed = listingDto.IsPetsAllowed,
                Status = listingDto.Status,
                HasParking=listingDto.HasParking
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
            existingListing.PropertyType = listingDto.PropertyType ?? existingListing.PropertyType;
            existingListing.ListingType = listingDto.ListingType ?? existingListing.ListingType;
            existingListing.IsPetsAllowed = listingDto.IsPetsAllowed ?? existingListing.IsPetsAllowed;
            existingListing.HasParking = listingDto.HasParking ?? existingListing.HasParking;
            existingListing.Status = listingDto.status ?? existingListing.Status;

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
        
        public async Task<PagedResult<PropertyListingResponseDto>> GetFilteredListingsAsync(PropertyListingQueryParametersDto query)
        {
            var listings = await _propertyListingRepository.GetAllAsync();

            // id
            // Filter by ListingId
            if (query.ListingId.HasValue)
                listings = listings.Where(l => l.Id == query.ListingId.Value);

            // Search
            if (!string.IsNullOrWhiteSpace(query.Keyword))
            {
                listings = listings.Where(l =>
                    l.Title.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase) ||
                    l.Description.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase)||
                    l.Location.Contains(query.Keyword,StringComparison.OrdinalIgnoreCase));
            }

            // Filter by AgentId
            if (query.AgentId.HasValue)
                listings = listings.Where(i => i.AgentId == query.AgentId.Value);

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

            if (!string.IsNullOrWhiteSpace(query.ListingType))
                listings = listings.Where(l => l.ListingType.Contains(query.ListingType, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.PropertyType))
                listings = listings.Where(l => l.PropertyType.Contains(query.PropertyType, StringComparison.OrdinalIgnoreCase));
            
            if (!string.IsNullOrWhiteSpace(query.Status))
                listings = listings.Where(l => l.Status.Contains(query.Status, StringComparison.OrdinalIgnoreCase));

            if (query.HasParking.HasValue)
                listings = listings.Where(l => l.HasParking == query.HasParking);
                
            if (query.IsPetsAllowed.HasValue)
                listings = listings.Where(l => l.IsPetsAllowed == query.IsPetsAllowed);

            // Sort
            listings = query.SortBy?.ToLower() switch
            {
                "price" => query.IsDescending ? listings.OrderByDescending(l => l.Price) : listings.OrderBy(l => l.Price),
                "bedrooms" => query.IsDescending ? listings.OrderByDescending(l => l.Bedrooms) : listings.OrderBy(l => l.Bedrooms),
                "bathrooms" => query.IsDescending ? listings.OrderByDescending(l => l.Bathrooms) : listings.OrderBy(l => l.Bathrooms),
                "createdAt" => query.IsDescending ? listings.OrderByDescending(l=>l.CreatedAt) : listings.OrderBy(l=>l.CreatedAt),
                _ => listings.OrderByDescending(l => l.CreatedAt)
            };

            // count for pagination
            int totalCount = listings.Count();

            // Pagination
            listings = listings
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            var listingDtos = new List<PropertyListingResponseDto>();
            foreach (var l in listings)
            {
                var imageUrls =( await _propertyImageService.GetImageUrlsByListingIdAsync(l.Id)).ToList();
                var agent = l.Agent;
                var user = agent?.User;
                listingDtos.Add(new PropertyListingResponseDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Description = l.Description,
                    Price = l.Price,
                    Location = l.Location,
                    Bedrooms = l.Bedrooms,
                    Bathrooms = l.Bathrooms,
                    SquareFeet = l.SquareFeet,
                    AgentId = l.AgentId,
                    PropertyType = l.PropertyType,
                    ListingType = l.ListingType,
                    IsPetsAllowed = l.IsPetsAllowed,
                    Status = l.Status,
                    HasParking=l.HasParking,
                    // Agent=l.Agent,
                    Name =user?.Name ?? string.Empty,
                    Email=user?.Email??string.Empty,
                    Phone=agent?.Phone ?? string.Empty,
                    AgencyName = agent?.AgencyName ?? string.Empty,
                    LicenseNumber = agent?.LicenseNumber ?? string.Empty,
                    ImageUrls = imageUrls,
                    CreatedAt = l.CreatedAt,
                    IsDeleted = l.IsDeleted
                });
            }

            return new PagedResult<PropertyListingResponseDto>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = listingDtos
            };
        }

    }
}