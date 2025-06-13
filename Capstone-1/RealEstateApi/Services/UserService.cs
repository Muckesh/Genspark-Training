using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public UserService(IRepository<Guid, User> userRepository, IPasswordService passwordService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<User> CreateAdminUser(CreateUserDto userDto)
        {
            var existing = await GetUserByEmail(userDto.Email);
            if (existing != null)
                throw new EmailAlreadyExistsException("User already exists with the given email.");

            string hashedPassword = _passwordService.HashPassword(userDto.Password);
            var refreshToken = await _tokenService.GenerateRefreshToken();

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Role = "Admin",
                PasswordHash = hashedPassword,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            user = await _userRepository.AddAsync(user);

            return user;
            
        }

        public async Task<User> UpdateUserAsync(Guid id, UpdateUserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null || user.IsDeleted)
                throw new UserNotFoundException("User not found.");

            if (!string.IsNullOrEmpty(userDto.Email))
            {
                var existing = await GetUserByEmail(userDto.Email);
                if (existing != null && existing.Id != id)
                    throw new EmailAlreadyExistsException("Email already in use.");
            }

            user.Name = userDto.Name ?? user.Name;
            user.Email = userDto.Email ?? user.Email;
            user.Role = userDto.Role ?? user.Role;

            return await _userRepository.UpdateAsync(id, user);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Email == email);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users;
        }

        public async Task<User> GetUserById(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user ?? throw new UserNotFoundException("No users found with the given id.");
        }

        public async Task<User> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            user.IsDeleted = true;
            user = await _userRepository.UpdateAsync(id, user);
            return user ?? throw new FailedOperationException("Unable to delete user at the moment.");
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.IsDeleted)
                throw new UserNotFoundException("User not found.");

            if (!_passwordService.VerifyPassword(dto.OldPassword, user.PasswordHash))
                throw new UnauthorizedAccessAppException("Old password is incorrect.");

            user.PasswordHash = _passwordService.HashPassword(dto.NewPassword);
            await _userRepository.UpdateAsync(userId, user);

            return true;
        }


        public async Task<PagedResult<User>> GetFilteredUsersAsync(UserQueryDto query)
        {
            var users = await _userRepository.GetAllAsync();

            // Filter
            if (!string.IsNullOrWhiteSpace(query.Name))
                users = users.Where(u => u.Name.Contains(query.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Email))
                users = users.Where(u => u.Email.Contains(query.Email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.Role))
                users = users.Where(u => u.Role.Equals(query.Role, StringComparison.OrdinalIgnoreCase));

            // Sorting
            users = query.SortBy?.ToLower() switch
            {
                "name" => query.IsDescending ? users.OrderByDescending(u => u.Name) : users.OrderBy(u => u.Name),
                "email" => query.IsDescending ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email),
                _ => users.OrderBy(u => u.Name)
            };

            int totalCount = users.Count();

            // Pagination
            users = users
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            return new PagedResult<User>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = users
            };
        }
        


    }
}