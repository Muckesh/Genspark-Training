using System.Security.Claims;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Mappers;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class BuyerService : IBuyerService
    {
        UserRegisterBuyerMapper userRegisterBuyerMapper;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IRepository<Guid, Buyer> _buyerRepository;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public BuyerService(IPasswordService passwordService, ITokenService tokenService, IRepository<Guid, User> userRepository, IRepository<Guid, Buyer> buyerRepository,IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            userRegisterBuyerMapper = new();
            _passwordService = passwordService;
            _tokenService = tokenService;
            _userRepository = userRepository;
            _buyerRepository = buyerRepository;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Buyer>> GetAllBuyers()
        {
            var buyers = await _buyerRepository.GetAllAsync();
            return buyers;
        }

        public async Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer)
        {
            
            var existing = await _userService.GetUserByEmail(registerBuyer.Email);
            if (existing != null)
                throw new EmailAlreadyExistsException("User already exists with the given email.");

            string hashedPassword = _passwordService.HashPassword(registerBuyer.Password);
            var refreshToken = await _tokenService.GenerateRefreshToken();


            var user = userRegisterBuyerMapper.MapUserBuyer(registerBuyer, hashedPassword, refreshToken);


            user = await _userRepository.AddAsync(user);

            

            var buyer = userRegisterBuyerMapper.BuyerUserMapper(registerBuyer, user);

            await _buyerRepository.AddAsync(buyer);

            var accessToken = await _tokenService.GenerateToken(user);


            return new AuthResponseDto
            {
                Email = user.Email,
                Role = user.Role,
                Token = accessToken,
                RefreshToken = refreshToken
            };

        }

        public async Task<PagedResult<Buyer>> GetFilteredBuyersAsync(BuyerQueryDto query)
        {
            var buyers = await _buyerRepository.GetAllAsync();

            // Filter by PreferredLocation
            if (!string.IsNullOrWhiteSpace(query.PreferredLocation))
            {
                buyers = buyers.Where(b =>
                    b.PreferredLocation != null &&
                    b.PreferredLocation.Contains(query.PreferredLocation, StringComparison.OrdinalIgnoreCase));
            }

            // Filter by Budget range
            if (query.MinBudget.HasValue)
                buyers = buyers.Where(b => b.Budget >= query.MinBudget.Value);

            if (query.MaxBudget.HasValue)
                buyers = buyers.Where(b => b.Budget <= query.MaxBudget.Value);

            // Sort
            buyers = query.SortBy?.ToLower() switch
            {
                "budget" => query.IsDescending ? buyers.OrderByDescending(b => b.Budget) : buyers.OrderBy(b => b.Budget),
                "location" => query.IsDescending ? buyers.OrderByDescending(b => b.PreferredLocation) : buyers.OrderBy(b => b.PreferredLocation),
                "name" => query.IsDescending ? buyers.OrderByDescending(b => b.User!.Name) : buyers.OrderBy(b => b.User!.Name),
                _ => buyers.OrderBy(b => b.Budget) // default sort
            };

            int totalCount = buyers.Count();

            // Pagination
            buyers = buyers
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            return new PagedResult<Buyer>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = buyers
            };
        }

        public async Task<Buyer> UpdateBuyerAsync(Guid buyerId, UpdateBuyerDto updateDto)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdStr, out var userId) || userId != buyerId)
                throw new UnauthorizedAccessAppException("You are not authorized to update this buyer profile.");

            var buyer = await _buyerRepository.GetByIdAsync(buyerId);
            if (buyer == null)
                throw new UserNotFoundException("Buyer not found.");

            buyer.PreferredLocation = updateDto.PreferredLocation ?? buyer.PreferredLocation;
            buyer.Budget = updateDto.Budget ?? buyer.Budget;

            var updatedBuyer = await _buyerRepository.UpdateAsync(buyer.Id, buyer);
            return updatedBuyer ?? throw new FailedOperationException("Failed to update buyer.");
        }


    }

}