using System;
using System.Security.Claims;
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
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);
        }

        [Fact]
        public async Task GetFilteredUsers_ReturnsOkResult()
        {
            var query = new UserQueryDto();
            _mockUserService.Setup(s => s.GetFilteredUsersAsync(query))
                .ReturnsAsync(new PagedResult<User>());

            var result = await _controller.GetFilteredUsers(query);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<PagedResult<User>>(okResult.Value);
        }

        [Fact]
        public async Task UpdateUser_ReturnsOkResult_WhenUserUpdated()
        {
            var id = Guid.NewGuid();
            var dto = new UpdateUserDto();
            var user = new User { Id = id };

            _mockUserService.Setup(s => s.UpdateUserAsync(id, dto)).ReturnsAsync(user);

            var result = await _controller.UpdateUser(id, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOk_WhenSuccess()
        {
            var id = Guid.NewGuid();
            var dto = new ChangePasswordDto();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            _mockUserService.Setup(s => s.ChangePasswordAsync(id, dto)).ReturnsAsync(true);

            var result = await _controller.ChangePassword(id, dto);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password changed successfully.", okResult.Value);
        }


        [Fact]
        public async Task ChangePassword_ReturnsForbid_WhenUnauthorized()
        {
            var id = Guid.NewGuid();
            var dto = new ChangePasswordDto();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            var result = await _controller.ChangePassword(id, dto);
            Assert.IsType<ForbidResult>(result);
        }
    }
}
