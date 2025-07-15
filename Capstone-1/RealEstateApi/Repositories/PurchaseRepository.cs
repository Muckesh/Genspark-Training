using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class PurchaseRepository : Repository<Guid, Purchase>
    {

        public PurchaseRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }

        public override async Task<IEnumerable<Purchase>> GetAllAsync()
        {
            var purchases = await _realEstateDbContext
                                    .Purchases
                                    .Include(p => p.Buyer)
                                    .Include(p => p.Listing)
                                    .ThenInclude(l => l.Agent)
                                    .ToListAsync();
            return purchases;
                                    
        }

        public override async Task<Purchase> GetByIdAsync(Guid id)
        {
            var purchase = await _realEstateDbContext
                                    .Purchases
                                    .Include(p => p.Buyer)
                                    .Include(p => p.Listing)
                                    .ThenInclude(l => l.Agent)
                                    .FirstOrDefaultAsync(p => p.Id == id);

            return purchase ?? throw new NotFoundException("Purchase not found");
        }
    }
}