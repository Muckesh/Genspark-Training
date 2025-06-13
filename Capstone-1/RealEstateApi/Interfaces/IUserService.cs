using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByEmail(string email);
        Task<PagedResult<User>> GetFilteredUsersAsync(UserQueryDto query);

        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User> GetUserById(Guid id);

        Task<User> DeleteUserAsync(Guid id);

        Task<User> CreateAdminUser(CreateUserDto userDto);

        Task<User> UpdateUserAsync(Guid id, UpdateUserDto userDto);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);


    }
}