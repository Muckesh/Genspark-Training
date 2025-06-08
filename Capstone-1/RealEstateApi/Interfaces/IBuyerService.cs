using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IBuyerService
    {
        Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer);
        Task<IEnumerable<Buyer>> GetAllBuyers();
    }
}