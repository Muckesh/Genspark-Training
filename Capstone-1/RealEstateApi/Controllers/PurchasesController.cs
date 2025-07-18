using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApi.Interfaces;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Buyer")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpPost]
        public async Task<IActionResult> BuyProperty([FromBody] CreatePurchaseDto dto)
        {
            try
            {
                var result = await _purchaseService.CreatePurchaseAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyPurchases()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var purchases = await _purchaseService.GetPurchasesByBuyerAsync(Guid.Parse(userId));
            return Ok(purchases);
        }

        [HttpPost("Buy")]//new endpoint with discountcode
        public async Task<IActionResult> BuyPropertyWithDiscountCode([FromBody] CreatePurchaseWithDiscountDto dto)
        {
            try
            {
                var result = await _purchaseService.CreatePurchaseWithDiscountAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}