using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentService _agentService;
        public AgentsController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpPost("register-agent")]
        public async Task<ActionResult<AuthResponseDto>> RegisterAgent(RegisterAgentDto agentDto)
        {
            try
            {
                var agent = await _agentService.RegisterAgentAsync(agentDto);
                return Ok(agent) ?? throw new Exception("Unable to register agent at the moment.");

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Agent>>> GetAgents()
        {
            var agents = await _agentService.GetAllAgents();
            return Ok(agents);
        }

    }
}