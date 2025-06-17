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
    public class PropertyListingsControllerTests
    {
        private readonly Mock<IPropertyListingService> _listingServiceMock;
        private readonly Mock<IPropertyImageService> _imageServiceMock;
        private readonly PropertyListingsController _controller;

        public PropertyListingsControllerTests()
        {
            _listingServiceMock = new Mock<IPropertyListingService>();
            _imageServiceMock = new Mock<IPropertyImageService>();
            _controller = new PropertyListingsController(_listingServiceMock.Object, _imageServiceMock.Object);
        }


        [Fact]
        public async Task GetListingById_ReturnsOk_WhenFound()
        {
            var listing = new PropertyListing { Id = Guid.NewGuid() };
            _listingServiceMock.Setup(s => s.GetListingByIdAsync(listing.Id)).ReturnsAsync(listing);

            var result = await _controller.GetListingById(listing.Id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateListing_ReturnsCreated_WhenValid()
        {
            var dto = new CreatePropertyListingDto();
            var listing = new PropertyListing { Id = Guid.NewGuid() };

            _listingServiceMock.Setup(s => s.AddListingAsync(dto)).ReturnsAsync(listing);
            var result = await _controller.CreateListing(dto);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task UploadImage_ReturnsOk_WhenSuccess()
        {
            var dto = new AddPropertyImageDto();
            var image = new PropertyImage { Id = Guid.NewGuid() };

            _imageServiceMock.Setup(s => s.UploadPropertyImageAsync(dto)).ReturnsAsync(image);

            var controllerWithContext = new PropertyListingsController(_listingServiceMock.Object, _imageServiceMock.Object);
            controllerWithContext.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await controllerWithContext.UploadImage(dto);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateListing_ReturnsUnauthorized_WhenNoAgentId()
        {
            var id = Guid.NewGuid();
            var dto = new UpdatePropertyListingDto();

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var controller = new PropertyListingsController(_listingServiceMock.Object, _imageServiceMock.Object)
            {
                ControllerContext = context
            };

            var result = await controller.UpdateListing(id, dto);
            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid user token.", unauthorized.Value);
        }

        [Fact]
        public async Task DeleteListing_ReturnsOk_WhenSuccess()
        {
            var id = Guid.NewGuid();
            var agentId = Guid.NewGuid();

            var listing = new PropertyListing { Id = id, AgentId = agentId, IsDeleted = false };
            _listingServiceMock.Setup(s => s.GetListingByIdAsync(id)).ReturnsAsync(listing);
            _listingServiceMock.Setup(s => s.DeleteListingAsync(id)).ReturnsAsync(listing);

            var context = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, agentId.ToString())
            }, "mock"));

            var controller = new PropertyListingsController(_listingServiceMock.Object, _imageServiceMock.Object)
            {
                ControllerContext = context
            };

            var result = await controller.DeleteListing(id);
            Assert.IsType<OkObjectResult>(result.Result);
        }
    }
}
