using RealEstateApi.Interaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using RealEstateApi.Repositories;

namespace RealEstateApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, Agent> _agentRepository;
        private readonly IRepository<Guid, Buyer> _buyerRepository;

        public AuthService(ITokenService tokenService,IRepository<Guid, User> userRepository, IRepository<Guid, Agent> agentRepository, IRepository<Guid, Buyer> buyerRepository)
        {
            _tokenService = tokenService;
            _userRepository = userRepository;
            _agentRepository = agentRepository;
            _buyerRepository = buyerRepository;
            
        }
        public Task<AuthResponseDto> LoginAsync(LoginDto login)
        {
            throw new NotImplementedException();
        }

        public async Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent)
        {
            // if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            // throw new Exception("Email already registered");

            var user = new User
            {
                Name = registerAgent.Name,
                Email = registerAgent.Email,
                Role = "Agent",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerAgent.Password)
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

        public async Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer)
        {
            // if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            // throw new Exception("Email already registered");

            var user = new User
            {
                Name = registerBuyer.Name,
                Email = registerBuyer.Email,
                Role = "Buyer",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerBuyer.Password)
            };

            await _userRepository.AddAsync(user);

            var buyer = new Buyer
            {
                Id = user.Id,
                PreferredLocation = registerBuyer.PreferredLocation,
                Budget=registerBuyer.Budget
            };

            await _buyerRepository.AddAsync(buyer);

            return new AuthResponseDto
            {
                Token = await _tokenService.GenerateToken(user),
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}