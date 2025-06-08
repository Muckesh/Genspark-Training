using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class AgentService : IAgentService
    {
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, Agent> _agentRepository;

        public AgentService(IPasswordService passwordService,ITokenService tokenService, IRepository<Guid, User> userRepository, IRepository<Guid, Agent> agentRepository)
        {
            _passwordService = passwordService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _agentRepository = agentRepository;

        }

        public async Task<IEnumerable<Agent>> GetAllAgents()
        {
            var agents = await _agentRepository.GetAllAsync();
            return agents;
        }

        public async Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent)
        {
            // if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            // throw new Exception("Email already registered");

            string hashedPassword = _passwordService.HashPassword(registerAgent.Password);
            var user = new User
            {
                Name = registerAgent.Name,
                Email = registerAgent.Email,
                Role = "Agent",
                PasswordHash = hashedPassword
            };

            await _userRepository.AddAsync(user);

            var agent = new Agent
            {
                Id = user.Id,
                LicenseNumber = registerAgent.LicenseNumber,
                AgencyName = registerAgent.AgencyName,
                Phone = registerAgent.Phone
            };

            await _agentRepository.AddAsync(agent);

            return new AuthResponseDto
            {
                Token = await _tokenService.GenerateToken(user),
                Email = user.Email,
                Role = user.Role
            };
        }

    }
}