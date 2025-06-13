using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/users/filter
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<User>>> GetFilteredUsers([FromQuery] UserQueryDto query)
        {
            try
            {
                var result = await _userService.GetFilteredUsersAsync(query);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // [HttpPost("create-admin")]
        // [AllowAnonymous]
        // // [Authorize(Roles ="Admin")]
        // public async Task<IActionResult> CreateAdmin([FromBody] CreateUserDto userDto)
        // {
        //     try
        //     {
        //         var existing = await _userService.GetUserByEmail(userDto.Email);
        //         if (existing != null)
        //             return BadRequest("User already exists.");

        //         var admin = await _userService.CreateAdminUser(userDto);

        //         return Created("", admin);
        //     }
        //     catch (Exception e)
        //     {
        //         return BadRequest(e.Message);
        //     }
        // }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(id, dto);
                return Ok(user);
            }
            catch (EmailAlreadyExistsException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("change-password/{id}")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto dto)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null || userId != id.ToString())
                    return Forbid("You can only change your own password.");

                var result = await _userService.ChangePasswordAsync(id, dto);
                return result ? Ok("Password changed successfully.") : BadRequest("Failed to change password.");
            }
            catch (UnauthorizedAccessAppException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var deletedUser = await _userService.DeleteUserAsync(id);
                return Ok(new
                {
                    message = "User deleted successfully",
                    userId = deletedUser.Id
                });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }

}