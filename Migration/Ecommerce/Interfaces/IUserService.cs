using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseDto> CreateUser(UserRequestDto user);
        Task<IEnumerable<UserResponseDto>> GetAllUsers();
        Task<UserResponseDto> GetUserById(int id);
        Task<UserResponseDto> UpdateUser(int id, UserUpdateRequestDto updateDto);
        Task<UserResponseDto> DeleteUser(int id);
    }
}