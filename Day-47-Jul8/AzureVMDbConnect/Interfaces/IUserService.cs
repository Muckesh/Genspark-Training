using AzureVMDbConnect.DTOs;
using AzureVMDbConnect.Models;

namespace AzureVMDbConnect.Interfaces
{
    public interface IUserService
    {
        Task<User> AddUser(UserDto userDto);
        ICollection<User> GetAll();
    }
}