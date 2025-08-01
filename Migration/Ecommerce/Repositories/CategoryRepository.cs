using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class CategoryRepository : Repository<int, Category>
    {
        public CategoryRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {
            
        }
        public override async Task<ICollection<Category>> GetAllAsync()
        {
            var categories = await _ecommerceDbContext.Categories.ToListAsync();
            return categories;
        }

        public override async Task<Category> GetByIdAsync(int key)
        {
            var category = await _ecommerceDbContext.Categories.SingleOrDefaultAsync(c => c.CategoryId == key);
            return category ?? throw new KeyNotFoundException("Category not found.");
        }
    }
}