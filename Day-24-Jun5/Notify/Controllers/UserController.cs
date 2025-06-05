// using System.Security.Claims;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;

// [ApiController]
// [Route("api/[controller]")]
// public class UserController : ControllerBase
// {
//     private readonly IRepository<string, User> _userRepository;
//     private readonly IEncryptionService _encryptionService;

//     public UserController(IRepository<string, User> userRepository, IEncryptionService encryptionService)
//     {
//         _userRepository = userRepository;
//         _encryptionService = encryptionService;
//     }

//     // Only HR Admins should be able to create new users
//     // [Authorize(Roles = "HRAdmin")]
//     [HttpPost("create")]
//     public async Task<IActionResult> CreateUser([FromBody] LoginRequestDto request)
//     {
//         var existingUser = await _userRepository.Get(request.Email);
//         if (existingUser != null)
//         {
//             return Conflict("User with this email already exists.");
//         }

//         var encrypted = await _encryptionService.EncryptData(new EncryptModel
//         {
//             Data = request.Password
//         });

//         var newUser = new User
//         {
//             Email = request.Email,
//             Password = encrypted.EncryptedData,
//             HashKey = encrypted.HashKey,
//             Role = "Staff" // Default role
//         };

//         await _userRepository.Add(newUser);
//         return Ok("User created successfully.");
//     }

//     // Only HR Admins can get all users
//     [Authorize(Roles = "HRAdmin")]
//     [HttpGet("all")]
//     public async Task<IActionResult> GetAllUsers()
//     {
//         var users = await _userRepository.GetAll();
//         var result = users.Select(u => new
//         {
//             u.Email,
//             u.Role
//         });

//         return Ok(result);
//     }

//     // Accessible to logged-in user to fetch their profile
//     [Authorize]
//     [HttpGet("me")]
//     public async Task<IActionResult> GetMyProfile()
//     {
//         var email = User.FindFirstValue(ClaimTypes.NameIdentifier);
//         var user = await _userRepository.Get(email);

//         if (user == null)
//             return NotFound("User not found.");

//         return Ok(new
//         {
//             user.Email,
//             user.Role
//         });
//     }

//     // HRAdmin updates user role
//     // [Authorize(Roles = "HRAdmin")]
//     [HttpPut("role")]
//     public async Task<IActionResult> UpdateRole(string email, string newRole)
//     {
//         var user = await _userRepository.Get(email);
//         if (user == null)
//             return NotFound("User not found.");

//         user.Role = newRole;
//         await _userRepository.Update(email, user);

//         return Ok($"Updated role of {email} to {newRole}.");
//     }

//     // HRAdmin deletes a user
//     [Authorize(Roles = "HRAdmin")]
//     [HttpDelete("{email}")]
//     public async Task<IActionResult> DeleteUser(string email)
//     {
//         try
//         {
//             await _userRepository.Delete(email);
//             return Ok("User deleted.");
//         }
//         catch
//         {
//             return NotFound("User not found.");
//         }
//     }
// }
