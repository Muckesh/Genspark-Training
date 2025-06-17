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
    public class AgentServiceTests
    {
        private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
        private readonly Mock<IRepository<Guid, Agent>> _agentRepoMock = new();
        private readonly Mock<IPasswordService> _passwordServiceMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IUserService> _userServiceMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

        private readonly AgentService _agentService;
        private readonly Guid _validAgentId = Guid.NewGuid();

        public AgentServiceTests()
        {
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _validAgentId.ToString())
            }));

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

            _agentService = new AgentService(
                _passwordServiceMock.Object,
                _tokenServiceMock.Object,
                _userRepoMock.Object,
                _agentRepoMock.Object,
                _userServiceMock.Object,
                _httpContextAccessorMock.Object
            );
        }

        [Fact]
        public async Task RegisterAgentAsync_ShouldThrow_WhenEmailAlreadyExists()
        {
            // Arrange
            var dto = new RegisterAgentDto { Email = "test@example.com" };
            _userServiceMock.Setup(s => s.GetUserByEmail(dto.Email)).ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<EmailAlreadyExistsException>(() => _agentService.RegisterAgentAsync(dto));
        }

        [Fact]
        public async Task RegisterAgentAsync_ShouldReturnAuthResponse_WhenValid()
        {
            // Arrange
            var dto = new RegisterAgentDto
            {
                Email = "test@example.com",
                Name = "Test",
                Password = "password",
                AgencyName = "TestAgency",
                Phone = "1234567890",
                LicenseNumber = "LIC123"
            };

            _userServiceMock.Setup(s => s.GetUserByEmail(dto.Email)).ReturnsAsync((User)null);
            _passwordServiceMock.Setup(s => s.HashPassword(dto.Password)).Returns("hashed");
            _tokenServiceMock.Setup(s => s.GenerateRefreshToken()).ReturnsAsync("refresh-token");
            _tokenServiceMock.Setup(s => s.GenerateToken(It.IsAny<User>())).ReturnsAsync("access-token");
            _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
            _agentRepoMock.Setup(r => r.AddAsync(It.IsAny<Agent>())).ReturnsAsync((Agent a) => a);

            // Act
            var result = await _agentService.RegisterAgentAsync(dto);

            // Assert
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal("Agent", result.Role);
            Assert.Equal("access-token", result.Token);
            Assert.Equal("refresh-token", result.RefreshToken);
        }

        [Fact]
        public async Task GetAllAgents_ShouldReturnListOfAgents()
        {
            // Arrange
            var agents = new List<Agent> { new Agent { Id = Guid.NewGuid() } };
            _agentRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(agents);

            // Act
            var result = await _agentService.GetAllAgents();

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task UpdateAgentAsync_ShouldThrow_WhenUnauthorized()
        {
            // Arrange
            var anotherAgentId = Guid.NewGuid(); // Not matching _validAgentId from HttpContext
            var updateDto = new UpdateAgentDto();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessAppException>(() => _agentService.UpdateAgentAsync(anotherAgentId, updateDto));
        }

        [Fact]
        public async Task UpdateAgentAsync_ShouldThrow_WhenAgentNotFound()
        {
            // Arrange
            var updateDto = new UpdateAgentDto();
            _agentRepoMock.Setup(r => r.GetByIdAsync(_validAgentId)).ReturnsAsync((Agent)null);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => _agentService.UpdateAgentAsync(_validAgentId, updateDto));
        }

        [Fact]
        public async Task UpdateAgentAsync_ShouldReturnUpdatedAgent()
        {
            // Arrange
            var updateDto = new UpdateAgentDto { AgencyName = "New", Phone = "9876543210", LicenseNumber = "NEWLIC" };
            var agent = new Agent { Id = _validAgentId };

            _agentRepoMock.Setup(r => r.GetByIdAsync(_validAgentId)).ReturnsAsync(agent);
            _agentRepoMock.Setup(r => r.UpdateAsync(_validAgentId, It.IsAny<Agent>())).ReturnsAsync(agent);

            // Act
            var result = await _agentService.UpdateAgentAsync(_validAgentId, updateDto);

            // Assert
            Assert.NotNull(result);
        }
    }
}
