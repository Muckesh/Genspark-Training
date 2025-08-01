using System.IdentityModel.Tokens.Jwt;
using Ecommerce.Interfaces;
using Ecommerce.Misc;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecommerce.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IPasswordService _passwordService;
        private readonly IRepository<int, User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(ITokenService tokenService, IPasswordService passwordService, IRepository<int, User> userRepository, ITokenBlacklistService tokenBlacklistService, IHttpContextAccessor httpContextAccessor)
        {
            _tokenService = tokenService;
            _tokenBlacklistService = tokenBlacklistService;
            _passwordService = passwordService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<User> GetUserDetailsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            // var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            if (userId == null)
                throw new Exception("Invalid or missing user Id.");
            var user = await _userRepository.GetByIdAsync(userId.Value);
            // if (userRole == "Buyer")
            // {
            //     var buyerRes = await _
            // }
            
            return user ?? throw new Exception("User not found.");
        }

        public async Task<AuthResponseDto> LoginAsync(AuthLoginRequestDto login)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.SingleOrDefault(u => u.Username == login.Username);
            if (user == null)
                throw new Exception("User does not exist.");
            bool isValid = _passwordService.VerifyPassword(login.Password, user.PasswordHash);
            if (!isValid)
                throw new Exception("Invalid Credentials");

            var accessToken = await _tokenService.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user = await _userRepository.UpdateAsync(user.UserId, user);

            return new AuthResponseDto
            {
                Username = user.Username,
                Role = user.Role,
                Token = accessToken,
                RefreshToken = refreshToken
            };

        }

        public async Task LogoutAsync(AuthLogoutRequestDto dto, string accessToken)
        {
            var user = await GetUserByRefreshToken(dto.RefreshToken);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userRepository.UpdateAsync(user.UserId, user);

            //  Blacklist the access token
            var jwtExpiry = GetTokenExpiry(accessToken); // parse JWT expiry from token
            await _tokenBlacklistService.AddToBlacklistAsync(accessToken, jwtExpiry);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(AuthRefreshTokenRequestDto refreshTokenRequestDto)
        {
            var user = await GetUserByRefreshToken(refreshTokenRequestDto.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token");

            var newAccessToken = await _tokenService.GenerateToken(user);
            var newRefreshToken = await _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user = await _userRepository.UpdateAsync(user.UserId, user);
            return new AuthResponseDto
            {
                Username = user.Username,
                Role = user.Role,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        private async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);
            return user ?? throw new Exception("User not found with the given refresh token.");
        }
        
        private DateTime GetTokenExpiry(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo; // in UTC
        }

    }
}