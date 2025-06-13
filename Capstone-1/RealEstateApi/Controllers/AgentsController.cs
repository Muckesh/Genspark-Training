using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentService _agentService;
        public AgentsController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpGet]
        // [Authorize(Roles = "Admin,Buyer")]
        public async Task<IActionResult> GetFilteredAgents([FromQuery] AgentQueryDto query)
        {
            try
            {
                var result = await _agentService.GetFilteredAgentsAsync(query);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> RegisterAgent(RegisterAgentDto agentDto)
        {
            try
            {
                // var user = await _userService.GetUserByEmail(agentDto.Email);
                // if (user != null)
                //     throw new Exception("User with the given email already exists.");
                var agent = await _agentService.RegisterAgentAsync(agentDto);
                return Created("", agent);

            }
            catch (EmailAlreadyExistsException e)
            {
                return Conflict(e.Message);
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }
        

        

        [Authorize(Roles = "Agent")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgent(Guid id, [FromBody] UpdateAgentDto updateDto)
        {
            try
            {
                var updatedAgent = await _agentService.UpdateAgentAsync(id, updateDto);
                return Ok(updatedAgent);
            }
            catch (UnauthorizedAccessAppException ex)
            {
                return Forbid(ex.Message);
            }
             catch (UserNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (FailedOperationException e)
            {
                return StatusCode(500, e.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}