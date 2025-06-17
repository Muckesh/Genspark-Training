using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public class AgentsControllerTests
    {
        private readonly Mock<IAgentService> _agentServiceMock;
        private readonly AgentsController _controller;

        public AgentsControllerTests()
        {
            _agentServiceMock = new Mock<IAgentService>();
            _controller = new AgentsController(_agentServiceMock.Object);
        }



        [Fact]
        public async Task RegisterAgent_ShouldReturnCreated_WhenSuccessful()
        {
            // Arrange
            var registerDto = new RegisterAgentDto
            {
                Name = "Agent Smith",
                Email = "smith@example.com",
                Password = "password123"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "token123",
                RefreshToken = "refresh456",
                Email = "smith@example.com",
                Role = "Agent"
            };

            _agentServiceMock.Setup(s => s.RegisterAgentAsync(registerDto)).ReturnsAsync(authResponse);

            // Act
            var result = await _controller.RegisterAgent(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result.Result);
            var response = Assert.IsType<AuthResponseDto>(createdResult.Value);
            Assert.Equal("Agent", response.Role);
            Assert.Equal("smith@example.com", response.Email);
        }


        [Fact]
        public async Task RegisterAgent_ShouldReturnConflict_WhenEmailExists()
        {
            // Arrange
            var registerDto = new RegisterAgentDto { Email = "exists@example.com" };

            _agentServiceMock
                .Setup(s => s.RegisterAgentAsync(registerDto))
                .ThrowsAsync(new EmailAlreadyExistsException("Email already exists."));

            // Act
            var result = await _controller.RegisterAgent(registerDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Equal("Email already exists.", conflictResult.Value);
        }

        [Fact]
        public async Task UpdateAgent_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var agentId = Guid.NewGuid();
            var updateDto = new UpdateAgentDto
            {
                AgencyName = "New Agency",
                Phone = "9876543210",
                LicenseNumber = "LIC12345"
            };

            var updatedAgent = new Agent
            {
                Id = agentId,
                AgencyName = "New Agency",
                Phone = "9876543210",
                LicenseNumber = "LIC12345"
            };

            _agentServiceMock.Setup(s => s.UpdateAgentAsync(agentId, updateDto)).ReturnsAsync(updatedAgent);

            // Act
            var result = await _controller.UpdateAgent(agentId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAgent = Assert.IsType<Agent>(okResult.Value);
            Assert.Equal("New Agency", returnedAgent.AgencyName);
            Assert.Equal("9876543210", returnedAgent.Phone);
            Assert.Equal("LIC12345", returnedAgent.LicenseNumber);
        }



        [Fact]
        public async Task UpdateAgent_ShouldReturnForbidden_WhenUnauthorized()
        {
            // Arrange
            var agentId = Guid.NewGuid();
            var updateDto = new UpdateAgentDto();

            _agentServiceMock
                .Setup(s => s.UpdateAgentAsync(agentId, updateDto))
                .ThrowsAsync(new UnauthorizedAccessAppException("Not allowed"));

            // Act
            var result = await _controller.UpdateAgent(agentId, updateDto);

            // Assert
            var forbidden = Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public async Task UpdateAgent_ShouldReturn500_WhenFailedOperation()
        {
            var agentId = Guid.NewGuid();
            var updateDto = new UpdateAgentDto();

            _agentServiceMock
                .Setup(s => s.UpdateAgentAsync(agentId, updateDto))
                .ThrowsAsync(new FailedOperationException("Internal failure"));

            var result = await _controller.UpdateAgent(agentId, updateDto);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal failure", statusCodeResult.Value);
        }
    }
}
