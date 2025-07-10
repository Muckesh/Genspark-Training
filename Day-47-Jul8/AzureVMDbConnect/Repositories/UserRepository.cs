using AzureVMDbConnect.Context;
using AzureVMDbConnect.Interfaces;
using AzureVMDbConnect.Models;

namespace AzureVMDbConnect.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserContext _userContext;
        public UserRepository(UserContext userContext)
        {
            _userContext = userContext;
        }
        public async Task<User> AddUserAsync(User user)
        {
            await _userContext.AddAsync(user);
            await _userContext.SaveChangesAsync();
            return user;
        }

        public ICollection<User> GetAllUsersAsync()
        {
            var users = _userContext.Users.ToList() ?? [];
            return users;
        }
    }
}