using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IBuyerService
    {
        Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer);
        Task<IEnumerable<Buyer>> GetAllBuyers();
        Task<PagedResult<Buyer>> GetFilteredBuyersAsync(BuyerQueryDto query);
        Task<Buyer> UpdateBuyerAsync(Guid buyerId, UpdateBuyerDto updateDto);

    }
}