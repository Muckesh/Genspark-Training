using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
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
                        .Include(a => a.User)
                        .Include(a => a.Listings)
                        .Where(a=>a.User!.IsDeleted==false)
                        .ToListAsync();
            return agents;
        }

        public override async Task<Agent> GetByIdAsync(Guid id)
        {
            var agent = await _realEstateDbContext.Agents
                            .Include(a => a.User)
                            .Include(a=>a.Listings)
                            .SingleOrDefaultAsync(a => a.User!.IsDeleted==false&& a.Id == id);
            return agent??throw new NotFoundException("Agent with the given ID not found.");
        }
    }
}