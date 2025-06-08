using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly IPasswordService _passwordService;
        private readonly IRepository<Guid, User> _userRepository;
       
        public AuthService(ITokenService tokenService,IPasswordService passwordService,IRepository<Guid, User> userRepository)
        {
            _tokenService = tokenService;
            _passwordService = passwordService;
            _userRepository = userRepository;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto login)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.SingleOrDefault(u => u.Email == login.Email);
            if (user == null)
                throw new Exception("User does not exist.");
            bool isValid = _passwordService.VerifyPassword(login.Password, user.PasswordHash);
            if (!isValid)
                throw new Exception("Invalid Credentials");

            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role,
                Token = await _tokenService.GenerateToken(user)
            };
        }

    
    }
}