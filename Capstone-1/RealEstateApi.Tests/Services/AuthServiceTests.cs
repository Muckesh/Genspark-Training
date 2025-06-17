using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using RealEstateApi.Services;
using Xunit;

namespace RealEstateApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
        private readonly Mock<IPasswordService> _passwordServiceMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<ITokenBlacklistService> _blacklistServiceMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService(
                _tokenServiceMock.Object,
                _passwordServiceMock.Object,
                _userRepoMock.Object,
                _blacklistServiceMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnAuthResponse_WhenCredentialsAreValid()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = "hashedPassword",
                Role = "Agent"
            };

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "plainPassword"
            };

            var users = new List<User> { user };

            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);
            _passwordServiceMock.Setup(p => p.VerifyPassword("plainPassword", "hashedPassword")).Returns(true);
            _tokenServiceMock.Setup(t => t.GenerateToken(user)).ReturnsAsync("access-token");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).ReturnsAsync("refresh-token");
            _userRepoMock.Setup(r => r.UpdateAsync(user.Id, It.IsAny<User>())).ReturnsAsync(user);

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.Equal(user.Email, result.Email);
            Assert.Equal("Agent", result.Role);
            Assert.Equal("access-token", result.Token);
            Assert.Equal("refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrow_WhenTokenIsExpired()
        {
            // Arrange
            var expiredUser = new User
            {
                Id = Guid.NewGuid(),
                RefreshToken = "old-refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1)
            };

            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { expiredUser });

            var dto = new RefreshTokenRequestDto { RefreshToken = "old-refresh-token" };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidCredentialsException>(() => _authService.RefreshTokenAsync(dto));
        }

        [Fact]
        public async Task LogoutAsync_ShouldClearRefreshTokenAndBlacklistAccessToken()
        {
            // Arrange
            var jwtToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(10));
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            var user = new User
            {
                Id = Guid.NewGuid(),
                RefreshToken = "logout-refresh-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { user });
            _userRepoMock.Setup(r => r.UpdateAsync(user.Id, It.IsAny<User>())).ReturnsAsync(user);
            _blacklistServiceMock.Setup(b => b.AddToBlacklistAsync(It.IsAny<string>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            var dto = new LogoutRequestDto { RefreshToken = "logout-refresh-token" };

            // Act
            await _authService.LogoutAsync(dto, token);

            // Assert
            _userRepoMock.Verify(r => r.UpdateAsync(user.Id, It.Is<User>(u => u.RefreshToken == null)), Times.Once);
            _blacklistServiceMock.Verify(b => b.AddToBlacklistAsync(token, It.IsAny<DateTime>()), Times.Once);
        }
    }
}
