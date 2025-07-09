using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
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
                                .ThenInclude(a=>a.User)
                            .Include(l => l.Images.Where(i=>i.IsDeleted==false))
                            .Include(l => l.Inquiries)
                            .Where(l=>l.IsDeleted==false)
                            .ToListAsync();
            // return listings.Count == 0 ? throw new Exception("No property listings in the database.") : listings;
            return listings;
        }

        public override async Task<PropertyListing> GetByIdAsync(Guid id)
        {
            var listing = await _realEstateDbContext.PropertyListings
                            .Include(l => l.Agent)
                                .ThenInclude(a=>a.User)
                            .Include(l => l.Images.Where(i=>i.IsDeleted==false))
                            .Include(l => l.Inquiries)
                            .FirstOrDefaultAsync(l => l.IsDeleted==false&& l.Id == id);
            return listing ?? throw new NotFoundException("Property listing not found");
        }
    }
}