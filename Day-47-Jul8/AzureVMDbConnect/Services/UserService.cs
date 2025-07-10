using AzureVMDbConnect.DTOs;
using AzureVMDbConnect.Interfaces;
using AzureVMDbConnect.Models;

namespace AzureVMDbConnect.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> AddUser(UserDto userDto)
        {
            try
            {
                var newUser = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Age = userDto.Age
                };
                var createdUser = await _userRepository.AddUserAsync(newUser);
                return createdUser;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public ICollection<User> GetAll()
        {
            try
            {
                return _userRepository.GetAllUsersAsync();
            }
            catch (Exception e)
            {
                
                throw;
            }
        }
    }
}