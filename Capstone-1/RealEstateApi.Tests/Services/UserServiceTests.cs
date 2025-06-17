using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using RealEstateApi.Services;
using Xunit;

namespace RealEstateApi.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IRepository<Guid, User>> _userRepoMock;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IRepository<Guid, User>>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _userService = new UserService(
                _userRepoMock.Object,
                _passwordServiceMock.Object,
                _tokenServiceMock.Object
            );
        }

        [Fact]
        public async Task CreateAdminUser_ShouldCreate_WhenEmailIsUnique()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Name = "John",
                Email = "john@example.com",
                Password = "password"
            };

            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            _passwordServiceMock.Setup(p => p.HashPassword(dto.Password)).Returns("hashed");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).ReturnsAsync("refresh_token");

            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

            // Act
            var user = await _userService.CreateAdminUser(dto);

            // Assert
            Assert.Equal("Admin", user.Role);
            Assert.Equal("hashed", user.PasswordHash);
            Assert.Equal("refresh_token", user.RefreshToken);
        }

        [Fact]
        public async Task CreateAdminUser_ShouldThrow_WhenEmailExists()
        {
            // Arrange
            var dto = new CreateUserDto
            {
                Name = "John",
                Email = "john@example.com",
                Password = "password"
            };

            var existingUser = new User { Email = dto.Email };
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User> { existingUser });

            // Act & Assert
            await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _userService.CreateAdminUser(dto));
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateFields()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingUser = new User
            {
                Id = userId,
                Name = "Old",
                Email = "old@example.com",
                Role = "Buyer"
            };

            var dto = new UpdateUserDto
            {
                Name = "New Name",
                Email = "new@example.com",
                Role = "Admin"
            };

            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<User>());
            _userRepoMock.Setup(r => r.UpdateAsync(userId, It.IsAny<User>())).ReturnsAsync((Guid _, User u) => u);

            // Act
            var result = await _userService.UpdateUserAsync(userId, dto);

            // Assert
            Assert.Equal("New Name", result.Name);
            Assert.Equal("new@example.com", result.Email);
            Assert.Equal("Admin", result.Role);
        }

        [Fact]
        public async Task DeleteUser_ShouldMarkAsDeleted()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, IsDeleted = false };

            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.UpdateAsync(userId, It.IsAny<User>())).ReturnsAsync((Guid _, User u) => u);

            var result = await _userService.DeleteUserAsync(userId);

            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task ChangePassword_ShouldSucceed_WhenOldPasswordMatches()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, PasswordHash = "oldhash" };

            var dto = new ChangePasswordDto
            {
                OldPassword = "old",
                NewPassword = "new"
            };

            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _passwordServiceMock.Setup(p => p.VerifyPassword(dto.OldPassword, user.PasswordHash)).Returns(true);
            _passwordServiceMock.Setup(p => p.HashPassword(dto.NewPassword)).Returns("newhash");

            var result = await _userService.ChangePasswordAsync(userId, dto);

            Assert.True(result);
            _userRepoMock.Verify(r => r.UpdateAsync(userId, It.Is<User>(u => u.PasswordHash == "newhash")), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_ShouldFail_WhenOldPasswordIsWrong()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, PasswordHash = "oldhash" };

            var dto = new ChangePasswordDto
            {
                OldPassword = "wrong",
                NewPassword = "new"
            };

            _userRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _passwordServiceMock.Setup(p => p.VerifyPassword(dto.OldPassword, user.PasswordHash)).Returns(false);

            await Assert.ThrowsAsync<UnauthorizedAccessAppException>(() => _userService.ChangePasswordAsync(userId, dto));
        }

        [Fact]
        public async Task GetFilteredUsersAsync_ShouldReturnPagedResults()
        {
            var users = new List<User>
            {
                new User { Name = "Alice", Email = "a@example.com", Role = "Admin" },
                new User { Name = "Bob", Email = "b@example.com", Role = "Buyer" },
                new User { Name = "Charlie", Email = "c@example.com", Role = "Buyer" }
            };

            _userRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

            var query = new UserQueryDto
            {
                PageNumber = 1,
                PageSize = 2,
                Role = "Buyer"
            };

            var result = await _userService.GetFilteredUsersAsync(query);

            Assert.Equal(2, result.PageSize);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, u => Assert.Equal("Buyer", u.Role));
        }
    }
}
