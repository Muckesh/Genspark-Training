using AzureVMDbConnect.DTOs;
using AzureVMDbConnect.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AzureVMDbConnect.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            try
            {
                var users = _userService.GetAll();
                return Ok(users);
            }
            catch (Exception)
            {

                return BadRequest("No users found.");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDto user)
        {
            try
            {
                var newUser = await _userService.AddUser(user);
                return Ok(newUser);
            }
            catch (Exception)
            {

                return BadRequest("Unable to add the user.");
            }
        }


    }
}