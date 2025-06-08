using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IAgentService
    {
        Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent);
        Task<IEnumerable<Agent>> GetAllAgents();
    }
}