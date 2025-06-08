using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto login);
    }
}