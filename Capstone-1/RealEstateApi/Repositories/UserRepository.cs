using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
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
            var users = await _realEstateDbContext.Users
            .Include(u => u.AgentProfile)
            .Include(u=>u.BuyerProfile)
            .Where(u => u.IsDeleted == false)
            .ToListAsync();
            // return users.Count == 0 ? throw new Exception("No users found") : users;
            return users;
        }

        public override async Task<User> GetByIdAsync(Guid id)
        {
            var user = await _realEstateDbContext.Users
            .Include(u => u.AgentProfile)
            .Include(u=>u.BuyerProfile)
            .SingleOrDefaultAsync(u => u.IsDeleted==false &&u.Id == id);
            return user ?? throw new UserNotFoundException("User not found");
        }
    }
}