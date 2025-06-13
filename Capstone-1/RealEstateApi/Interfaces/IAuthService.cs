using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto login);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);

        Task LogoutAsync(LogoutRequestDto dto, string accessToken);
        Task<User> GetUserDetailsAsync();

    }
}