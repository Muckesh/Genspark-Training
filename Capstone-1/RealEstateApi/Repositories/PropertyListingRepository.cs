using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class PropertyListingRepository : Repository<Guid, PropertyListing>
    {
        public PropertyListingRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }

        public override async Task<IEnumerable<PropertyListing>> GetAllAsync()
        {
            var listings = await _realEstateDbContext.PropertyListings
                            .Include(l => l.Agent)
                            .Include(l => l.Images)
                            .Include(l=>l.Inquiries)
                            .ToListAsync();
            return listings.Count == 0 ? throw new Exception("No property listings in the database.") : listings;
        }

        public override async Task<PropertyListing> GetByIdAsync(Guid id)
        {
            var listing = await _realEstateDbContext.PropertyListings
                            .Include(l => l.Agent)
                            .Include(l => l.Images)
                            .Include(l => l.Inquiries)
                            .SingleOrDefaultAsync(l => l.Id == id);
            return listing ?? throw new Exception("Property listing not found");
        }
    }
}