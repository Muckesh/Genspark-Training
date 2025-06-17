using System;
using System.Collections.Generic;
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
    public class InquiryServiceTests
    {
        private readonly Mock<IRepository<Guid, Inquiry>> _inquiryRepoMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly InquiryService _service;

        public InquiryServiceTests()
        {
            _inquiryRepoMock = new Mock<IRepository<Guid, Inquiry>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _service = new InquiryService(_inquiryRepoMock.Object, _httpContextAccessorMock.Object);
        }

        private void SetupHttpContext(string userId, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext { User = user };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);
        }

        [Fact]
        public async Task CreateInquiryAsync_ShouldSucceed_ForBuyer()
        {
            // Arrange
            var buyerId = Guid.NewGuid();
            SetupHttpContext(buyerId.ToString(), "Buyer");

            var dto = new AddInquiryDto
            {
                ListingId = Guid.NewGuid(),
                Message = "I'm interested"
            };

            var expectedInquiry = new Inquiry
            {
                Id = Guid.NewGuid(),
                BuyerId = buyerId,
                ListingId = dto.ListingId,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow
            };

            _inquiryRepoMock.Setup(r => r.AddAsync(It.IsAny<Inquiry>())).ReturnsAsync(expectedInquiry);

            // Act
            var result = await _service.CreateInquiryAsync(dto);

            // Assert
            Assert.Equal(expectedInquiry.Id, result.Id);
            Assert.Equal(buyerId, result.BuyerId);
        }

        [Fact]
        public async Task CreateInquiryAsync_ShouldThrow_WhenAdminProvidesBuyerId()
        {
            // Arrange
            SetupHttpContext(Guid.NewGuid().ToString(), "Admin");

            var dto = new AddInquiryDto
            {
                ListingId = Guid.NewGuid(),
                BuyerId = Guid.NewGuid(), // should not be set by admin
                Message = "Admin inquiry"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentRequiredException>(() => _service.CreateInquiryAsync(dto));
        }

        [Fact]
        public async Task CreateInquiryAsync_ShouldThrow_WhenUnauthorizedRole()
        {
            // Arrange
            SetupHttpContext(Guid.NewGuid().ToString(), "Agent");

            var dto = new AddInquiryDto
            {
                ListingId = Guid.NewGuid(),
                Message = "Unauthorized role"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessAppException>(() => _service.CreateInquiryAsync(dto));
        }

        [Fact]
        public async Task CreateInquiryAsync_ShouldThrow_WhenUserContextMissing()
        {
            // Arrange
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns((HttpContext)null);

            var dto = new AddInquiryDto
            {
                ListingId = Guid.NewGuid(),
                Message = "Invalid context"
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessAppException>(() => _service.CreateInquiryAsync(dto));
        }

        [Fact]
        public async Task GetFilteredInquiriesAsync_ShouldReturnPagedResults()
        {
            // Arrange
            var data = new List<Inquiry>
            {
                new Inquiry { Id = Guid.NewGuid(), BuyerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow.AddDays(-1), Message = "Test 1" },
                new Inquiry { Id = Guid.NewGuid(), BuyerId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, Message = "Test 2" }
            };

            _inquiryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(data);

            var query = new InquiryQueryDto
            {
                PageNumber = 1,
                PageSize = 10,
                Keyword = "Test"
            };

            // Act
            var result = await _service.GetFilteredInquiriesAsync(query);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }
    }
}
