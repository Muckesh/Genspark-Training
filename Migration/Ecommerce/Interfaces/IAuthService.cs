using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(AuthLoginRequestDto login);
        Task<AuthResponseDto> RefreshTokenAsync(AuthRefreshTokenRequestDto refreshTokenRequestDto);

        Task LogoutAsync(AuthLogoutRequestDto dto, string accessToken);
        Task<User> GetUserDetailsAsync();

    }
}