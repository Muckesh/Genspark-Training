using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.HttpResults;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IPasswordService _passwordService;
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ITokenService tokenService, IPasswordService passwordService, IRepository<Guid, User> userRepository, ITokenBlacklistService tokenBlacklistService, IHttpContextAccessor httpContextAccessor)
        {
            _tokenService = tokenService;
            _tokenBlacklistService = tokenBlacklistService;
            _passwordService = passwordService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.RefreshToken == refreshToken);
            return user ?? throw new UserNotFoundException("User not found with the given refresh token.");
        }
       
        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            var user = await GetUserByRefreshToken(refreshTokenRequestDto.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new InvalidCredentialsException("Invalid or expired refresh token");

            var newAccessToken = await _tokenService.GenerateToken(user);
            var newRefreshToken = await _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            user = await _userRepository.UpdateAsync(user.Id, user);
            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role,
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto login)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.SingleOrDefault(u => u.Email == login.Email);
            if (user == null)
                throw new UserNotFoundException("User does not exist.");
            bool isValid = _passwordService.VerifyPassword(login.Password, user.PasswordHash);
            if (!isValid)
                throw new InvalidCredentialsException("Invalid Credentials");

            var accessToken = await _tokenService.GenerateToken(user);
            var refreshToken = await _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            user = await _userRepository.UpdateAsync(user.Id, user);

            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role,
                Token = accessToken,
                RefreshToken = refreshToken
            };

        }

        public async Task LogoutAsync(LogoutRequestDto dto, string accessToken)
        {
            var user = await GetUserByRefreshToken(dto.RefreshToken);

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userRepository.UpdateAsync(user.Id, user);

            //  Blacklist the access token
            var jwtExpiry = GetTokenExpiry(accessToken); // parse JWT expiry from token
            await _tokenBlacklistService.AddToBlacklistAsync(accessToken, jwtExpiry);
        }

        
        private DateTime GetTokenExpiry(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            return jwt.ValidTo; // in UTC
        }

        public async Task<User> GetUserDetailsAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();
            if (userId == null)
                throw new UnauthorizedAccessAppException("Invalid or missing user Id.");
            var user = await _userRepository.GetByIdAsync(userId.Value);
            return user ?? throw new UserNotFoundException("User not found.");
        }
    }
}