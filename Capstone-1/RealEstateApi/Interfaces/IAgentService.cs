using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IAgentService
    {
        Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent);
        Task<IEnumerable<Agent>> GetAllAgents();
        Task<PagedResult<Agent>> GetFilteredAgentsAsync(AgentQueryDto query);
        Task<Agent> UpdateAgentAsync(Guid agentId, UpdateAgentDto updateDto);


    }
}