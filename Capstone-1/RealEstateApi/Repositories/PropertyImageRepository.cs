using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
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
                            .Where(p=>p.IsDeleted==false)
                            .ToListAsync();
            // return images.Count == 0 ? throw new Exception("No property images found.") : images;
            return images;
        }

        public override async Task<PropertyImage> GetByIdAsync(Guid id)
        {
            var image = await _realEstateDbContext.PropertyImages
                            .Include(p => p.Listing)
                            .SingleOrDefaultAsync(p => p.IsDeleted == false && p.Id == id);
            return image ?? throw new NotFoundException("Image not found.");
        }
    }
}