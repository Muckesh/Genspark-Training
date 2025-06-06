using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class PropertyImageRepository : Repository<Guid, PropertyImage>
    {
        public PropertyImageRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }

        public override async Task<IEnumerable<PropertyImage>> GetAllAsync()
        {
            var images = await _realEstateDbContext.PropertyImages
                            .Include(p => p.Listing)
                            .ToListAsync();
            return images.Count == 0 ? throw new Exception("No property images found.") : images;
        }

        public override async Task<PropertyImage> GetByIdAsync(Guid id)
        {
            var image = await _realEstateDbContext.PropertyImages
                            .Include(p => p.Listing)
                            .SingleOrDefaultAsync(p => p.Id == id);
            return image ?? throw new Exception("Image not found.");
        }
    }
}