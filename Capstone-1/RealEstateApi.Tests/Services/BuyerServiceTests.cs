using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Mappers;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using RealEstateApi.Services;
using Xunit;

namespace RealEstateApi.Tests.Services
{
    public class BuyerServiceTests
    {
        private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
        private readonly Mock<IRepository<Guid, Buyer>> _buyerRepoMock = new();
        private readonly Mock<IPasswordService> _passwordServiceMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

        private readonly BuyerService _buyerService;

        public BuyerServiceTests()
        {
            _buyerService = new BuyerService(
                _passwordServiceMock.Object,
                _tokenServiceMock.Object,
                _userRepoMock.Object,
                _buyerRepoMock.Object,
                _userServiceMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Fact]
        public async Task RegisterBuyerAsync_ShouldRegisterNewBuyerAndReturnAuthResponse()
        {
            // Arrange
            var registerDto = new RegisterBuyerDto
            {
                Name = "John",
                Email = "john@example.com",
                Password = "secure",
                Budget = 100000,
                PreferredLocation = "Chennai"
            };

            _userServiceMock.Setup(s => s.GetUserByEmail(registerDto.Email)).ReturnsAsync((User?)null);
            _passwordServiceMock.Setup(p => p.HashPassword("secure")).Returns("hashed");
            _tokenServiceMock.Setup(t => t.GenerateRefreshToken()).ReturnsAsync("refresh-token");
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
            _buyerRepoMock.Setup(r => r.AddAsync(It.IsAny<Buyer>())).ReturnsAsync((Buyer b) => b);
            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).ReturnsAsync("access-token");

            // Act
            var result = await _buyerService.RegisterBuyerAsync(registerDto);

            // Assert
            Assert.Equal("john@example.com", result.Email);
            Assert.Equal("Buyer", result.Role);
            Assert.Equal("access-token", result.Token);
            Assert.Equal("refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task GetFilteredBuyersAsync_ShouldReturnFilteredAndPaginatedResult()
        {
            // Arrange
            var buyers = new List<Buyer>
            {
                new Buyer { Id = Guid.NewGuid(), Budget = 80000, PreferredLocation = "Chennai", User = new User { Name = "Alice" } },
                new Buyer { Id = Guid.NewGuid(), Budget = 120000, PreferredLocation = "Bangalore", User = new User { Name = "Bob" } }
            };

            _buyerRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(buyers);

            var query = new BuyerQueryDto
            {
                PreferredLocation = "chen",
                MinBudget = 70000,
                MaxBudget = 90000,
                SortBy = "name",
                IsDescending = false,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _buyerService.GetFilteredBuyersAsync(query);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("Chennai", result.Items.First().PreferredLocation);
        }

        [Fact]
        public async Task UpdateBuyerAsync_ShouldUpdateBuyer_WhenAuthorized()
        {
            // Arrange
            var buyerId = Guid.NewGuid();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, buyerId.ToString())
            }));

            _httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(claimsPrincipal);

            var buyer = new Buyer
            {
                Id = buyerId,
                Budget = 100000,
                PreferredLocation = "Old City"
            };

            _buyerRepoMock.Setup(r => r.GetByIdAsync(buyerId)).ReturnsAsync(buyer);
            _buyerRepoMock.Setup(r => r.UpdateAsync(buyerId, It.IsAny<Buyer>())).ReturnsAsync((Guid _, Buyer b) => b);

            var updateDto = new UpdateBuyerDto
            {
                Budget = 150000,
                PreferredLocation = "New City"
            };

            // Act
            var result = await _buyerService.UpdateBuyerAsync(buyerId, updateDto);

            // Assert
            Assert.Equal("New City", result.PreferredLocation);
            Assert.Equal(150000, result.Budget);
        }
    }
}
