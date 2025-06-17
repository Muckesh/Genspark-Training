using System;
using System.Collections.Generic;
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
    public class BuyersControllerTests
    {
        private readonly Mock<IBuyerService> _buyerServiceMock;
        private readonly BuyersController _controller;

        public BuyersControllerTests()
        {
            _buyerServiceMock = new Mock<IBuyerService>();
            _controller = new BuyersController(_buyerServiceMock.Object);
        }

        [Fact]
        public async Task GetFilteredBuyers_ReturnsOkResult_WithPagedBuyers()
        {
            // Arrange
            var query = new BuyerQueryDto { PageNumber = 1, PageSize = 10 };
            var pagedResult = new PagedResult<Buyer> { PageNumber = 1, PageSize = 10, TotalCount = 1, Items = new List<Buyer> { new Buyer() } };

            _buyerServiceMock.Setup(s => s.GetFilteredBuyersAsync(query)).ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetFilteredBuyers(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<PagedResult<Buyer>>(okResult.Value);
        }

        [Fact]
        public async Task RegisterBuyer_ReturnsCreatedResult_WithAuthResponseDto()
        {
            // Arrange
            var dto = new RegisterBuyerDto { Email = "buyer@test.com", Password = "pass" };
            var response = new AuthResponseDto { Email = dto.Email, Role = "Buyer", Token = "token", RefreshToken = "refresh" };

            _buyerServiceMock.Setup(s => s.RegisterBuyerAsync(dto)).ReturnsAsync(response);

            // Act
            var result = await _controller.RegisterBuyer(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            var returned = Assert.IsType<AuthResponseDto>(createdResult.Value);
            Assert.Equal(dto.Email, returned.Email);
        }

        [Fact]
        public async Task RegisterBuyer_WhenEmailExists_ReturnsConflict()
        {
            // Arrange
            var dto = new RegisterBuyerDto { Email = "buyer@test.com", Password = "pass" };
            _buyerServiceMock.Setup(s => s.RegisterBuyerAsync(dto))
                .ThrowsAsync(new EmailAlreadyExistsException("Email already exists"));

            // Act
            var result = await _controller.RegisterBuyer(dto);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result.Result); // Fixed here
            Assert.Equal("Email already exists", conflict.Value);
        }


        [Fact]
        public async Task UpdateBuyer_ValidBuyer_ReturnsOkResult()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var updateDto = new UpdateBuyerDto { Budget = 100000 };
            var updatedBuyer = new Buyer { Id = buyerId, Budget = 100000 };

            _buyerServiceMock.Setup(s => s.UpdateBuyerAsync(buyerId, updateDto)).ReturnsAsync(updatedBuyer);

            // Act
            var result = await _controller.UpdateBuyer(buyerId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var buyer = Assert.IsType<Buyer>(okResult.Value);
            Assert.Equal(100000, buyer.Budget);
        }

        [Fact]
        public async Task UpdateBuyer_Unauthorized_ThrowsForbid()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            var updateDto = new UpdateBuyerDto { Budget = 100000 };

            _buyerServiceMock.Setup(s => s.UpdateBuyerAsync(buyerId, updateDto))
                .ThrowsAsync(new UnauthorizedAccessAppException("Unauthorized"));

            // Act
            var result = await _controller.UpdateBuyer(buyerId, updateDto);

            // Assert
            var forbid = Assert.IsType<ForbidResult>(result);
        }
    }
}
