using RealEstateApi.Interaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, AgentRepository> _agentRepository;
        private readonly IRepository<Guid, Buyer> _buyerRepository;
        public Task<AuthResponseDto> LoginAsync(LoginDto login)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer)
        {
            throw new NotImplementedException();
        }
    }
}