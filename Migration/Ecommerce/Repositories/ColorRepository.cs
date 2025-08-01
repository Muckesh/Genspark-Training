using Ecommerce.Models;
using Ecommerce.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class ColorRepository : Repository<int, Color>
    {
        public ColorRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {

        }

        public override async Task<ICollection<Color>> GetAllAsync()
        {
            var colors = await _ecommerceDbContext.Colors.ToListAsync();
            return colors;
        }

        public override async Task<Color> GetByIdAsync(int key)
        {
            var color = await _ecommerceDbContext.Colors.SingleOrDefaultAsync(c => c.ColorId == key);
            return color ?? throw new KeyNotFoundException("Color not found.");
        }
    }
}