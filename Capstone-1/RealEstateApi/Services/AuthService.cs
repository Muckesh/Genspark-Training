using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
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
        private readonly IEmailSender _emailSender;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly IPasswordService _passwordService;
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, PasswordResetToken> _passwordResetTokenRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(ITokenService tokenService, IEmailSender emailSender,IRepository<Guid, PasswordResetToken> passwordResetTokenRepository, IPasswordService passwordService, IRepository<Guid, User> userRepository, ITokenBlacklistService tokenBlacklistService, IHttpContextAccessor httpContextAccessor)
        {
            _tokenService = tokenService;
            _emailSender = emailSender;
            _tokenBlacklistService = tokenBlacklistService;
            _passwordResetTokenRepository = passwordResetTokenRepository;
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
            if (user.IsDeleted == true)
                throw new UserNotFoundException("You have been disabled by admin. Please contact the admin : admin@gmail.com");
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

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = (await _userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Email == dto.Email);

            if (user == null) return; // Don't expose user existence

            var token = Guid.NewGuid().ToString();
            var resetToken = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                Expiry = DateTime.UtcNow.AddHours(1)
            };

            await _passwordResetTokenRepository.AddAsync(resetToken);

            // using var context = _httpContextAccessor.HttpContext!.RequestServices
            //     .GetRequiredService<RealEstateDbContext>();
            // context.PasswordResetTokens.Add(resetToken);
            // await context.SaveChangesAsync();

            var resetLink = $"http://localhost:4200/reset-password?token={token}";
            await _emailSender.SendAsync(user.Email, "Reset Your Password", $"<p>Click <a href=\"{resetLink}\">here</a> to reset your password.</p>");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            // using var context = _httpContextAccessor.HttpContext!.RequestServices
            //     .GetRequiredService<RealEstateDbContext>();

            // var tokenEntry = await context.PasswordResetTokens
            //     .Include(t => t.User)
            //     .FirstOrDefaultAsync(t => t.Token == dto.Token && t.Expiry > DateTime.UtcNow);

            var tokenEntries = await _passwordResetTokenRepository.GetAllAsync();
            var tokenEntry = tokenEntries.SingleOrDefault(t => t.Token == dto.Token && t.Expiry > DateTime.UtcNow);

            if (tokenEntry == null)
                throw new InvalidCredentialsException("Invalid or expired token.");

            tokenEntry.User!.PasswordHash = _passwordService.HashPassword(dto.NewPassword);

            // context.PasswordResetTokens.Remove(tokenEntry);
            // await context.SaveChangesAsync();
            await _passwordResetTokenRepository.DeleteAsync(tokenEntry.Id);
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
            // var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
            if (userId == null)
                throw new UnauthorizedAccessAppException("Invalid or missing user Id.");
            var user = await _userRepository.GetByIdAsync(userId.Value);
            // if (userRole == "Buyer")
            // {
            //     var buyerRes = await _
            // }
            
            return user ?? throw new UserNotFoundException("User not found.");
        }
    }
}