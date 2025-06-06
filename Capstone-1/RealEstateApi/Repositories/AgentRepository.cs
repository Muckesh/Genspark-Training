using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Repositories;

namespace RealEstateApi.Models
{
    public class AgentRepository : Repository<Guid, Agent>
    {
        public AgentRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }
        public override async Task<IEnumerable<Agent>> GetAllAsync()
        {
            var agents = await _realEstateDbContext.Agents
                        .Include(a=>a.User)
                        .ToListAsync();
            return agents.Count == 0 ? throw new Exception("No agents in the database.") : agents;
        }

        public override async Task<Agent> GetByIdAsync(Guid id)
        {
            var agent = await _realEstateDbContext.Agents
                            .Include(a=>a.User)
                            .SingleOrDefaultAsync(a => a.Id == id);
            return agent??throw new Exception("Agent with the given ID not found.");
        }
    }
}