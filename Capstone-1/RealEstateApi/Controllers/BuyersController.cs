using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BuyersController : ControllerBase
    {
        private readonly IBuyerService _buyerService;
        public BuyersController(IBuyerService buyerService)
        {
            _buyerService = buyerService;
        }

        [HttpPost("register-buyer")]
        public async Task<ActionResult<AuthResponseDto>> RegisterBuyer(RegisterBuyerDto buyerDto)
        {
            try
            {
                var buyer = await _buyerService.RegisterBuyerAsync(buyerDto);
                return Ok(buyer) ?? throw new Exception("Unable to register buyer at the moment.");

            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Buyer>>> GetBuyers()
        {
            var buyers = await _buyerService.GetAllBuyers();
            return Ok(buyers);
        }

    }
}