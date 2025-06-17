using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using RealEstateApi.Exceptions;
using RealEstateApi.Hubs;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using RealEstateApi.Services;
using Xunit;

namespace RealEstateApi.Tests.Services
{
    public class PropertyListingServiceTests
    {
        private readonly Mock<IRepository<Guid, PropertyListing>> _propertyRepoMock;
        private readonly Mock<IRepository<Guid, PropertyImage>> _imageRepoMock;
        private readonly Mock<IHubContext<NotificationHub>> _hubMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly PropertyListingService _service;

        public PropertyListingServiceTests()
        {
            _propertyRepoMock = new Mock<IRepository<Guid, PropertyListing>>();
            _imageRepoMock = new Mock<IRepository<Guid, PropertyImage>>();
            _hubMock = new Mock<IHubContext<NotificationHub>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var clientsMock = new Mock<IHubClients>();
            var clientProxyMock = new Mock<IClientProxy>();
            clientsMock.Setup(c => c.All).Returns(clientProxyMock.Object);
            _hubMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            _service = new PropertyListingService(
                _hubMock.Object,
                _propertyRepoMock.Object,
                _imageRepoMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        private void SetHttpContext(string role, Guid? userId = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role),
            };

            if (userId.HasValue)
                claims.Add(new Claim(ClaimTypes.NameIdentifier, userId.ToString()));

            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext { User = principal };
            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);
        }

        [Fact]
        public async Task AddListingAsync_AsAgent_ReturnsListing()
        {
            // Arrange
            var agentId = Guid.NewGuid();
            SetHttpContext("Agent", agentId);

            var dto = new CreatePropertyListingDto
            {
                Title = "Test Home",
                Description = "Nice house",
                Price = 100000,
                Location = "City",
                Bedrooms = 2,
                Bathrooms = 1
            };

            _propertyRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<PropertyListing>());

            _propertyRepoMock.Setup(r => r.AddAsync(It.IsAny<PropertyListing>()))
                .ReturnsAsync((PropertyListing listing) => listing);

            // Act
            var result = await _service.AddListingAsync(dto);

            // Assert
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(agentId, result.AgentId);
        }

        [Fact]
        public async Task UpdateListingAsync_ValidId_UpdatesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var existing = new PropertyListing
            {
                Id = id,
                Title = "Old",
                Price = 100000,
                IsDeleted = false
            };

            var dto = new UpdatePropertyListingDto
            {
                Title = "New Title"
            };

            _propertyRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
            _propertyRepoMock.Setup(r => r.UpdateAsync(id, It.IsAny<PropertyListing>()))
                .ReturnsAsync((Guid _, PropertyListing l) => l);

            // Act
            var result = await _service.UpdateListingAsync(id, dto);

            // Assert
            Assert.Equal("New Title", result.Title);
        }

        [Fact]
        public async Task DeleteListingAsync_ValidId_DeletesSuccessfully()
        {
            // Arrange
            var id = Guid.NewGuid();
            var listing = new PropertyListing { Id = id, IsDeleted = false };

            _propertyRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(listing);
            _propertyRepoMock.Setup(r => r.UpdateAsync(id, It.IsAny<PropertyListing>()))
                .ReturnsAsync((Guid _, PropertyListing l) => l);

            _imageRepoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<PropertyImage>
                {
                    new PropertyImage { Id = Guid.NewGuid(), PropertyListingId = id, IsDeleted = false }
                });

            _imageRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PropertyImage>()))
                .ReturnsAsync((Guid id, PropertyImage img) => img);

            // Act
            var result = await _service.DeleteListingAsync(id);

            // Assert
            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task GetFilteredListingsAsync_FiltersAndPaginates()
        {
            // Arrange
            var listings = Enumerable.Range(1, 20).Select(i =>
                new PropertyListing
                {
                    Title = $"Home {i}",
                    Description = "Beautiful house",
                    Price = i * 10000,
                    Bedrooms = i % 5,
                    Bathrooms = i % 3,
                    Location = "City"
                }).ToList();

            var query = new PropertyListingQueryParametersDto
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "price",
                IsDescending = true
            };

            _propertyRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(listings);

            // Act
            var result = await _service.GetFilteredListingsAsync(query);

            // Assert
            Assert.Equal(10, result.Items.Count());
            Assert.Equal(20, result.TotalCount);
            Assert.Equal(200000, result.Items.First().Price);
        }
    }
}
