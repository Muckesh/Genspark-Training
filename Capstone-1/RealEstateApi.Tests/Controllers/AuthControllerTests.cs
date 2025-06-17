using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RealEstateApi.Controllers;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using Xunit;

namespace RealEstateApi.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "user@test.com", Password = "password" };
            var authResponse = new AuthResponseDto { Email = "user@test.com", Token = "token123", Role = "Buyer" };

            _authServiceMock.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal("user@test.com", response.Email);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsInvalid()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "user@test.com", Password = "wrong" };
            _authServiceMock.Setup(x => x.LoginAsync(loginDto)).ThrowsAsync(new InvalidCredentialsException("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid credentials", unauthorizedResult.Value);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnOk_WhenTokenIsValid()
        {
            var refreshRequest = new RefreshTokenRequestDto
            {
                RefreshToken = "refresh_token"
            };
            var authResponse = new AuthResponseDto
            {
                Email = "user@test.com",
                Token = "new_token",
                Role = "Buyer"
            };

            _authServiceMock.Setup(x => x.RefreshTokenAsync(refreshRequest)).ReturnsAsync(authResponse);

            var result = await _controller.RefreshToken(refreshRequest);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(okResult.Value);
            Assert.Equal("new_token", response.Token);
        }

        [Fact]
        public async Task Logout_ShouldReturnOk_WhenSuccessful()
        {
            var logoutRequest = new LogoutRequestDto { RefreshToken = "refresh_token" };
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Bearer dummyAccessToken";
            _controller.ControllerContext = new ControllerContext { HttpContext = context };

            _authServiceMock
                .Setup(x => x.LogoutAsync(logoutRequest, "dummyAccessToken"))
                .Returns(Task.CompletedTask);

            var result = await _controller.Logout(logoutRequest);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Logout Successful.", okResult.Value);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnOk_WhenUserExists()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "user@test.com", Role = "Buyer" };
            _authServiceMock.Setup(x => x.GetUserDetailsAsync()).ReturnsAsync(user);

            var result = await _controller.GetCurrentUser();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal("user@test.com", returnedUser.Email);
        }

        [Fact]
        public async Task GetCurrentUser_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
        {
            _authServiceMock
                .Setup(x => x.GetUserDetailsAsync())
                .ThrowsAsync(new UnauthorizedAccessAppException("Unauthorized"));

            var result = await _controller.GetCurrentUser();

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Unauthorized", unauthorizedResult.Value);
        }
    }
}
