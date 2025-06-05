
using Microsoft.EntityFrameworkCore;

public class UserRepository : Repository<string, User>
{
    public UserRepository(NotifyDbContext context) : base(context)
    {
        
    }
    public override async Task<User> Get(string key)
    {
        return await _context.Users.SingleOrDefaultAsync(u => u.UserName == key) ?? throw new Exception("User not found with the given id.");
    }

    public override async Task<IEnumerable<User>> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return users.Count ==0 ? throw new Exception("No users in the database.") : users;
    }
}