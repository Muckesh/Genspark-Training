using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class OrderRepository : Repository<int, Order>
    {
        public OrderRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {
            
        }

        public override async Task<ICollection<Order>> GetAllAsync()
        {
            var orders = await _ecommerceDbContext
                                .Orders
                                .Include(o=>o.OrderDetails)
                                .ToListAsync();
            return orders;
        }

        public override async Task<Order> GetByIdAsync(int key)
        {
            var order = await _ecommerceDbContext.Orders.SingleOrDefaultAsync(o => o.OrderID == key);
            return order ?? throw new KeyNotFoundException("Order not found.");
        }
    }
}