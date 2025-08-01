using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class UserRepository : Repository<int, User>
    {
        public UserRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {

        }

        public override async Task<ICollection<User>> GetAllAsync()
        {
            var users = await _ecommerceDbContext.Users.ToListAsync();
            return users;
        }

        public override async Task<User> GetByIdAsync(int key)
        {
            var user = await _ecommerceDbContext.Users.SingleOrDefaultAsync(u => u.UserId == key);
            return user ?? throw new KeyNotFoundException("User not found.");
        }
    }
}