using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Interaces;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register-agent")]
        public async Task<IActionResult> RegisterAgent([FromBody] RegisterAgentDto registerAgentDto)
        {
            var result = await _authService.RegisterAgentAsync(registerAgentDto);
            return Ok(result);
        }

        [HttpPost("register-buyer")]
        public async Task<IActionResult> RegisterBuyer([FromBody] RegisterBuyerDto registerBuyerDto)
        {
            var result = await _authService.RegisterBuyerAsync(registerBuyerDto);
            return Ok(result);
        }

        // [HttpPost("login")]
        // public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        // {
            
        // }
    }
}