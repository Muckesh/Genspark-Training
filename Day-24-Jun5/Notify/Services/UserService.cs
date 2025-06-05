// public class UserService : IUserService
// {
//     private readonly IRepository<string, User> _userRepo;
//     private readonly IEncryptionService _encryptionService;
//     private readonly ITokenService _tokenService;

//     public UserService(IRepository<string, User> userRepo, IEncryptionService encryptionService, ITokenService tokenService)
//     {
//         _userRepo = userRepo;
//         _encryptionService = encryptionService;
//         _tokenService = tokenService;
//     }

//     public async Task<LoginResponseDto> Login(LoginRequestDto request)
//     {
//         var user = await _userRepo.Get(request.Email);

//         if (user == null || user.Password == null || user.HashKey == null)
//             throw new UnauthorizedAccessException("Invalid credentials.");

//         var encrypted = await _encryptionService.EncryptData(new EncryptModel
//         {
//             Data = request.Password,
//             HashKey = user.HashKey
//         });

//         bool isPasswordMatch = encrypted.EncryptedData.SequenceEqual(user.Password);

//         if (!isPasswordMatch)
//             throw new UnauthorizedAccessException("Invalid credentials.");

//         var token = await _tokenService.GenerateToken(user);

//         return new LoginResponseDto
//         {
//             Email = user.Email,
//             Role = user.Role,
//             Token = token
//         };
//     }
// }
