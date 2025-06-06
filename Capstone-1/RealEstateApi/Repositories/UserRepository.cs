using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class UserRepository : Repository<Guid, User>
    {
        public UserRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            var users = await _realEstateDbContext.Users.ToListAsync();
            return users.Count == 0 ? throw new Exception("No users found") : users;
        }

        public override async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _realEstateDbContext.Users.SingleOrDefaultAsync(u => u.Id == id);
            return user ?? throw new Exception("User not found");
        }
    }
}