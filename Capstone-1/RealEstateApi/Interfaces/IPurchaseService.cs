using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IPurchaseService
    {
        Task<Purchase> CreatePurchaseAsync(CreatePurchaseDto purchaseDto);
        Task<IEnumerable<Purchase>> GetPurchasesByBuyerAsync(Guid buyerId);
    }
}