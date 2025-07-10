using AzureVMDbConnect.Models;

namespace AzureVMDbConnect.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        ICollection<User> GetAllUsersAsync();
    }
}