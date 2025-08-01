using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class OrderDetailRepository : Repository<int, OrderDetail>
    {
        public OrderDetailRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {

        }
        public override async Task<ICollection<OrderDetail>> GetAllAsync()
        {
            var orderDetails = await _ecommerceDbContext.OrderDetails.ToListAsync();
            return orderDetails;
        }

        public override async Task<OrderDetail> GetByIdAsync(int key)
        {
            var orderDetail = await _ecommerceDbContext.OrderDetails.SingleOrDefaultAsync(od => od.OrderID == key);
            return orderDetail ?? throw new KeyNotFoundException("Order Detail not found");
        }
        
        public async Task<OrderDetail> GetByProductIdAsync(int key)
        {
            var orderDetail = await _ecommerceDbContext.OrderDetails.SingleOrDefaultAsync(od => od.ProductID == key);
            return orderDetail ?? throw new KeyNotFoundException("Order Detail not found");
        }
    }
}