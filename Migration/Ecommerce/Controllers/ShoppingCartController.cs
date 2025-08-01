using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] CheckoutDto checkoutDto)
        {
            if (checkoutDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var orderResponse = await _shoppingCartService.PlaceOrderAsync(checkoutDto);
            // return CreatedAtAction(nameof(PlaceOrder), new { id = orderResponse.OrderID }, orderResponse);
            return Ok(orderResponse);
        }

        // [HttpPost("place-order/paypal")]
        // public async Task<IActionResult> PlaceOrderPaypal([FromBody] CheckoutDto checkoutDto)
        // {
        //     if (checkoutDto == null || !ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     var orderResponse = await _shoppingCartService.PlaceOrderPaypalAsync(checkoutDto);
        //     // return CreatedAtAction(nameof(PlaceOrder), new { id = orderResponse.OrderID }, orderResponse);
        //     return Ok(orderResponse);
        // }
    }
}