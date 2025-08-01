using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class ModelRepository : Repository<int, Model>
    {
        public ModelRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {
            
        }
        public override async Task<ICollection<Model>> GetAllAsync()
        {
            var models = await _ecommerceDbContext.Models.ToListAsync();
            return models;
        }

        public override async Task<Model> GetByIdAsync(int key)
        {
            var models = await _ecommerceDbContext.Models.SingleOrDefaultAsync(m => m.ModelId == key);
            return models ?? throw new KeyNotFoundException("Model not found.");
        }
    }
}