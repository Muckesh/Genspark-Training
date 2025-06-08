using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class BuyerService : IBuyerService
    {
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, Buyer> _buyerRepository;

        public BuyerService(IPasswordService passwordService, ITokenService tokenService, IRepository<Guid, User> userRepository, IRepository<Guid, Buyer> buyerRepository)
        {
            _passwordService = passwordService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _buyerRepository = buyerRepository;
        }

        public async Task<IEnumerable<Buyer>> GetAllBuyers()
        {
            var buyers = await _buyerRepository.GetAllAsync();
            return buyers;
        }

        public async Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer)
        {
            // if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            // throw new Exception("Email already registered");

            string hashedPassword = _passwordService.HashPassword(registerBuyer.Password);
            var user = new User
            {
                Name = registerBuyer.Name,
                Email = registerBuyer.Email,
                PasswordHash = hashedPassword,
                Role = "Buyer"
            };

            user = await _userRepository.AddAsync(user);

            var buyer = new Buyer
            {
                Id = user.Id,
                PreferredLocation = registerBuyer.PreferredLocation,
                Budget = registerBuyer.Budget
            };

            buyer = await _buyerRepository.AddAsync(buyer);

            return new AuthResponseDto
            {
                Token = await _tokenService.GenerateToken(user),
                Email = user.Email,
                Role = user.Role
            };
        }
    }

}