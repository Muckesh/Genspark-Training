using System;
using System.Collections.Generic;
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
using RealEstateApi.Misc;
using Xunit;

namespace RealEstateApi.Tests
{
    public class PropertyListingServiceTests
    {
        private readonly Mock<IRepository<Guid, PropertyListing>> _propertyListingRepoMock = new();
        private readonly Mock<IRepository<Guid, PropertyImage>> _propertyImageRepoMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

        private readonly PropertyListingService _service;

        public PropertyListingServiceTests()
        {
            _service = new PropertyListingService(
                _propertyListingRepoMock.Object,
                _propertyImageRepoMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Fact]
        public async Task AddListingAsync_AdminWithoutAgentId_ThrowsException()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(claimsPrincipal);

            var dto = new CreatePropertyListingDto
            {
                Title = "Test Home",
                Location = "Chennai",
                Price = 100000,
                Bedrooms = 2,
                Bathrooms = 1
                // AgentId = null
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentRequiredException>(() => _service.AddListingAsync(dto));
        }

        [Fact]
        public async Task AddListingAsync_DuplicateListing_ThrowsException()
        {
            // Arrange
            var agentId = Guid.NewGuid();
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Agent"),
                new Claim(ClaimTypes.NameIdentifier, agentId.ToString())
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext!.User).Returns(claimsPrincipal);

            var dto = new CreatePropertyListingDto
            {
                Title = "Luxury Villa",
                Location = "Chennai",
                Price = 500000,
                Bedrooms = 3,
                Bathrooms = 2
            };

            var existing = new List<PropertyListing>
            {
                new PropertyListing
                {
                    Title = "Luxury Villa",
                    Location = "Chennai",
                    Price = 500000,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    AgentId = agentId,
                    IsDeleted = false
                }
            };

            _propertyListingRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(existing);

            // Act & Assert
            await Assert.ThrowsAsync<FailedOperationException>(() => _service.AddListingAsync(dto));
        }

        [Fact]
        public async Task UpdateListingAsync_ValidData_UpdatesListing()
        {
            // Arrange
            var listingId = Guid.NewGuid();
            var existing = new PropertyListing
            {
                Id = listingId,
                Title = "Old Title",
                IsDeleted = false,
                Price = 100000
            };

            var updateDto = new UpdatePropertyListingDto
            {
                Title = "New Title",
                Price = 120000
            };

            _propertyListingRepoMock.Setup(r => r.GetByIdAsync(listingId)).ReturnsAsync(existing);
            _propertyListingRepoMock.Setup(r => r.UpdateAsync(listingId, It.IsAny<PropertyListing>())).ReturnsAsync((Guid _, PropertyListing l) => l);

            // Act
            var updated = await _service.UpdateListingAsync(listingId, updateDto);

            // Assert
            Assert.Equal("New Title", updated.Title);
            Assert.Equal(120000, updated.Price);
        }

        [Fact]
        public async Task DeleteListingAsync_ShouldSoftDeleteListingAndImages()
        {
            // Arrange
            var listingId = Guid.NewGuid();

            var listing = new PropertyListing
            {
                Id = listingId,
                IsDeleted = false
            };

            var images = new List<PropertyImage>
            {
                new PropertyImage { Id = Guid.NewGuid(), PropertyListingId = listingId, IsDeleted = false },
                new PropertyImage { Id = Guid.NewGuid(), PropertyListingId = listingId, IsDeleted = false }
            };

            _propertyListingRepoMock.Setup(r => r.GetByIdAsync(listingId)).ReturnsAsync(listing);
            _propertyListingRepoMock.Setup(r => r.UpdateAsync(listingId, It.IsAny<PropertyListing>())).ReturnsAsync(listing);
            _propertyImageRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(images);
            _propertyImageRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Guid>(), It.IsAny<PropertyImage>())).ReturnsAsync((Guid _, PropertyImage img) => img);

            // Act
            var deletedListing = await _service.DeleteListingAsync(listingId);

            // Assert
            Assert.True(deletedListing.IsDeleted);
            foreach (var img in images)
            {
                Assert.True(img.IsDeleted);
                Assert.NotNull(img.DeletedAt);
            }
        }

        [Fact]
        public async Task GetFilteredListingsAsync_ShouldReturnPaginatedAndFilteredResults()
        {
            // Arrange
            var listings = new List<PropertyListing>
            {
                new PropertyListing { Title = "Sea View", Description = "Great place", Location = "Chennai", Price = 150000, Bedrooms = 2, Bathrooms = 1 },
                new PropertyListing { Title = "Hill View", Description = "Peaceful stay", Location = "Ooty", Price = 200000, Bedrooms = 3, Bathrooms = 2 }
            };

            _propertyListingRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(listings);

            var query = new PropertyListingQueryParametersDto
            {
                Keyword = "view",
                MinPrice = 100000,
                MaxPrice = 180000,
                PageNumber = 1,
                PageSize = 10
            };

            // Act
            var result = await _service.GetFilteredListingsAsync(query);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("Sea View", result.Items.First().Title);
        }
    }
}
