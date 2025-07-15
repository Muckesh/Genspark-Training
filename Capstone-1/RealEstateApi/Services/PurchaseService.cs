using System.Security.Claims;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<Guid, Purchase> _purchaseRepository;
        private readonly IRepository<Guid, PropertyListing> _listingRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PurchaseService(IRepository<Guid, Purchase> purchaseRepository,
                            IRepository<Guid, PropertyListing> listingRepository,
                            IHttpContextAccessor httpContextAccessor)
        {
            _purchaseRepository = purchaseRepository;
            _listingRepository = listingRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Purchase> CreatePurchaseAsync(CreatePurchaseDto purchaseDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var role = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            if (role != "Buyer" || !userId.HasValue)
                throw new UnauthorizedAccessAppException("Only buyers can purchase properties.");

            var listing = await _listingRepository.GetByIdAsync(purchaseDto.ListingId);
            if (listing == null || listing.IsDeleted)
                throw new NotFoundException("Listing not found.");

            if (listing.Status == "Sold")
                throw new FailedOperationException("This property has already been sold.");

            // Mark listing as sold
            listing.Status = "Sold";
            listing.UpdatedAt = DateTime.UtcNow;
            await _listingRepository.UpdateAsync(listing.Id, listing);

            var purchase = new Purchase
            {
                BuyerId = userId.Value,
                ListingId = listing.Id,
                PriceAtPurchase = listing.Price
            };

            return await _purchaseRepository.AddAsync(purchase);

        }

        public async Task<IEnumerable<Purchase>> GetPurchasesByBuyerAsync(Guid buyerId)
        {
            var purchases = await _purchaseRepository.GetAllAsync();
            return purchases.Where(p => p.BuyerId == buyerId);
        }
    }
}