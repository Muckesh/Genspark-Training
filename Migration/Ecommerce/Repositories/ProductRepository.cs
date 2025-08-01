using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class ProductRepository : Repository<int, Product>
    {
        public ProductRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {
            
        }
        public override async Task<ICollection<Product>> GetAllAsync()
        {
            var products = await _ecommerceDbContext
                                .Products
                                .Include(p=>p.Category)
                                .ToListAsync();
            return products;
        }

        public override async Task<Product> GetByIdAsync(int key)
        {
            var product = await _ecommerceDbContext.Products.SingleOrDefaultAsync(p => p.ProductId == key);
            return product ?? throw new KeyNotFoundException("Product not found.");
        }
    }
}