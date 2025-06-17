using System;
using System.Collections.Generic;
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
    public class InquiriesControllerTests
    {
        private readonly Mock<IInquiryService> _inquiryServiceMock;
        private readonly InquiriesController _controller;

        public InquiriesControllerTests()
        {
            _inquiryServiceMock = new Mock<IInquiryService>();
            _controller = new InquiriesController(_inquiryServiceMock.Object);
        }

        [Fact]
        public async Task GetFilteredInquiries_ShouldReturnOk_WithResults()
        {
            // Arrange
            var query = new InquiryQueryDto();
            var expected = new PagedResult<Inquiry>
            {
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 1,
                Items = new List<Inquiry> { new Inquiry { Id = Guid.NewGuid(), Message = "Test Inquiry" } }
            };

            _inquiryServiceMock.Setup(x => x.GetFilteredInquiriesAsync(query)).ReturnsAsync(expected);

            // Act
            var result = await _controller.GetFilteredInquiries(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<PagedResult<Inquiry>>(okResult.Value);
            Assert.Single(data.Items);
        }

        [Fact]
        public async Task GetFilteredInquiries_ShouldReturnBadRequest_OnException()
        {
            // Arrange
            var query = new InquiryQueryDto();
            _inquiryServiceMock
                .Setup(x => x.GetFilteredInquiriesAsync(query))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetFilteredInquiries(query);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Database error", badRequest.Value);
        }

        [Fact]
        public async Task AddInquiry_ShouldReturnCreated_OnSuccess()
        {
            // Arrange
            var dto = new AddInquiryDto { Message = "Interested", ListingId = Guid.NewGuid() };
            var inquiry = new Inquiry { Id = Guid.NewGuid(), Message = "Interested" };

            _inquiryServiceMock.Setup(x => x.CreateInquiryAsync(dto)).ReturnsAsync(inquiry);

            // Act
            var result = await _controller.AddInquiry(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var returnedInquiry = Assert.IsType<Inquiry>(createdResult.Value);
            Assert.Equal("Interested", returnedInquiry.Message);
        }

        [Fact]
        public async Task AddInquiry_ShouldReturnForbid_WhenUnauthorized()
        {
            var dto = new AddInquiryDto();
            _inquiryServiceMock
                .Setup(x => x.CreateInquiryAsync(dto))
                .ThrowsAsync(new UnauthorizedAccessAppException("Unauthorized"));

            var result = await _controller.AddInquiry(dto);

            var forbidResult = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task AddInquiry_ShouldReturnBadRequest_WhenMissingBuyerId()
        {
            var dto = new AddInquiryDto();
            _inquiryServiceMock
                .Setup(x => x.CreateInquiryAsync(dto))
                .ThrowsAsync(new ArgumentRequiredException("BuyerId required"));

            var result = await _controller.AddInquiry(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("BuyerId required", badRequest.Value);
        }

        [Fact]
        public async Task AddInquiry_ShouldReturn500_WhenCreationFails()
        {
            var dto = new AddInquiryDto();
            _inquiryServiceMock
                .Setup(x => x.CreateInquiryAsync(dto))
                .ThrowsAsync(new FailedOperationException("DB failure"));

            var result = await _controller.AddInquiry(dto);

            var error = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, error.StatusCode);
            Assert.Equal("DB failure", error.Value);
        }

        [Fact]
        public async Task AddInquiry_ShouldReturnBadRequest_OnGenericException()
        {
            var dto = new AddInquiryDto();
            _inquiryServiceMock
                .Setup(x => x.CreateInquiryAsync(dto))
                .ThrowsAsync(new Exception("Unhandled"));

            var result = await _controller.AddInquiry(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unhandled", badRequest.Value);
        }
    }
}
