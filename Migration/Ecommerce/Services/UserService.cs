using Ecommerce.Interfaces;
using Ecommerce.Models;
using Ecommerce.Models.DTOs;

namespace Ecommerce.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<int, User> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public UserService(IRepository<int, User> userRepository, IPasswordService passwordService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }
        public async Task<UserResponseDto> CreateUser(UserRequestDto user)
        {
            var users = await _userRepository.GetAllAsync();
            var existing = users.SingleOrDefault(u => string.Equals(u.Username, user.Username, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                throw new Exception("User Already exists. Try Loggin in.");
            }

            string hashedPassword = _passwordService.HashPassword(user.Password);
            var refreshToken = await _tokenService.GenerateRefreshToken();

            var newUser = new User
            {
                Username = user.Username,
                PasswordHash = hashedPassword,
                Role = user.Role,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };
            newUser = await _userRepository.AddAsync(newUser);
            return new UserResponseDto
            {
                UserId = newUser.UserId,
                Username = newUser.Username,
                Role = newUser.Role
            };
        }

        public async Task<UserResponseDto> DeleteUser(int id)
        {
            var user = await _userRepository.DeleteAsync(id);
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            var usersList = new List<UserResponseDto>();
            foreach (var user in users)
            {
                var userResponse = new UserResponseDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Role = user.Role
                };
                usersList.Add(userResponse);
            }
            return usersList;
        }

        public async Task<UserResponseDto> GetUserById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role

            };
        }

        public async Task<UserResponseDto> UpdateUser(int id, UserUpdateRequestDto updateDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            user.Role = updateDto.Role;
            var updatedUser = await _userRepository.UpdateAsync(id, user);
            return new UserResponseDto
            {
                UserId = updatedUser.UserId,
                Username = updatedUser.Username,
                Role = updatedUser.Role
            };

        }
    }
}