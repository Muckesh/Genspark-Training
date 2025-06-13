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
    public class BuyersController : ControllerBase
    {
        private readonly IBuyerService _buyerService;
        public BuyersController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        [HttpGet]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PagedResult<Buyer>>> GetFilteredBuyers([FromQuery] BuyerQueryDto query)
        {
            try
            {
                var result = await _buyerService.GetFilteredBuyersAsync(query);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> RegisterBuyer(RegisterBuyerDto buyerDto)
        {
            try
            {
                // var user = await _userService.GetUserByEmail(buyerDto.Email);
                // if (user != null)
                //     throw new Exception("User with the given email already exists.");
                var buyer = await _buyerService.RegisterBuyerAsync(buyerDto);
                return Created("", buyer);

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


        
        
        [Authorize(Roles = "Buyer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBuyer(Guid id, [FromBody] UpdateBuyerDto updateDto)
        {
            try
            {
                var updatedBuyer = await _buyerService.UpdateBuyerAsync(id, updateDto);
                return Ok(updatedBuyer);
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