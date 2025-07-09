using System.Security.Claims;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Mappers;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class AgentService : IAgentService
    {

        UserRegisterAgentMapper userRegisterAgentMapper;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, Agent> _agentRepository;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        

        public AgentService(IPasswordService passwordService, ITokenService tokenService, IRepository<Guid, User> userRepository, IRepository<Guid, Agent> agentRepository, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            userRegisterAgentMapper = new();
            _passwordService = passwordService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _agentRepository = agentRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<IEnumerable<Agent>> GetAllAgents()
        {
            var agents = await _agentRepository.GetAllAsync();
            return agents;
        }

        public async Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent)
        {
            
            var existing = await _userService.GetUserByEmail(registerAgent.Email);
            if (existing != null)
                throw new EmailAlreadyExistsException("User already exists with the given email.");

            string hashedPassword = _passwordService.HashPassword(registerAgent.Password);
            var refreshToken = await _tokenService.GenerateRefreshToken();

            

            var user = userRegisterAgentMapper.MapUserAgent(registerAgent, hashedPassword, refreshToken);

            await _userRepository.AddAsync(user);

            

            var agent = userRegisterAgentMapper.AgentUserMapper(registerAgent, user);

            await _agentRepository.AddAsync(agent);

            var accessToken = await _tokenService.GenerateToken(user);
            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role,
                Token = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<PagedResult<Agent>> GetFilteredAgentsAsync(AgentQueryDto query)
        {
            var agents = await _agentRepository.GetAllAsync();

            // Filter by Name
            if (!string.IsNullOrWhiteSpace(query.Name))
                agents = agents.Where(a => a.User!.Name.StartsWith(query.Name, StringComparison.OrdinalIgnoreCase));

            // Filter by AgencyName
            if (!string.IsNullOrWhiteSpace(query.AgencyName))
                agents = agents.Where(a => a.AgencyName.Contains(query.AgencyName, StringComparison.OrdinalIgnoreCase));

            // Filter by Phone
            if (!string.IsNullOrWhiteSpace(query.Phone))
                agents = agents.Where(a => a.Phone.Contains(query.Phone, StringComparison.OrdinalIgnoreCase));

            
            if (!string.IsNullOrWhiteSpace(query.Email))
                agents = agents.Where(a => a.User!.Email.Contains(query.Email, StringComparison.OrdinalIgnoreCase));


            // Sorting
            agents = query.SortBy?.ToLower() switch
            {
                "name" => query.IsDescending ? agents.OrderByDescending(a => a.User!.Name) : agents.OrderBy(a => a.User!.Name),
                "agencyname" => query.IsDescending ? agents.OrderByDescending(a => a.AgencyName) : agents.OrderBy(a => a.AgencyName),
                "phone" => query.IsDescending ? agents.OrderByDescending(a => a.Phone) : agents.OrderBy(a => a.Phone),
                _ => agents.OrderBy(a => a.User!.Name)
            };

            int totalCount = agents.Count();

            // Pagination
            agents = agents
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            return new PagedResult<Agent>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = agents
            };
        }

        public async Task<Agent> UpdateAgentAsync(Guid agentId, UpdateAgentDto updateDto)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.GetUserId();

            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;


            if (userRole != "Admin")
            {
                if ( userId != agentId)
                    throw new UnauthorizedAccessAppException("You are not authorized to update this buyer profile.");
            }

            var agent = await _agentRepository.GetByIdAsync(agentId);
            if (agent == null)
                throw new UserNotFoundException("Agent not found.");

            agent.AgencyName = updateDto.AgencyName ?? agent.AgencyName;
            agent.Phone = updateDto.Phone ?? agent.Phone;
            agent.LicenseNumber = updateDto.LicenseNumber ?? agent.LicenseNumber;

            var updatedAgent = await _agentRepository.UpdateAsync(agent.Id, agent);
            return updatedAgent ?? throw new FailedOperationException("Failed to update agent.");
        }


    }
}