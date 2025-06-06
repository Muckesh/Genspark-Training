using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent);
        Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer);

        Task<AuthResponseDto> LoginAsync(LoginDto login);
    }
}