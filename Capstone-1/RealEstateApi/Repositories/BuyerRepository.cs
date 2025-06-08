using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class BuyerRepository : Repository<Guid, Buyer>
    {
        public BuyerRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }

        public override async Task<IEnumerable<Buyer>> GetAllAsync()
        {
            var buyers = await _realEstateDbContext.Buyers
                            .Include(b => b.User)
                            .ToListAsync();
            // return buyers.Count == 0 ? throw new Exception("No buyers in the database.") : buyers;
            return buyers;
        }

        public override async Task<Buyer> GetByIdAsync(Guid id)
        {
            var buyer = await _realEstateDbContext.Buyers
                            .Include(b=>b.User)
                            .SingleOrDefaultAsync(b => b.Id == id);
            return buyer ?? throw new Exception("Buyer with the given Id not found.");
        }
    }
}