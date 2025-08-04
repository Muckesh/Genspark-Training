using Ecommerce.Interfaces;
using Ecommerce.Models.DTOs;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly PaypalService _paypalService;

        public ShoppingCartController(IShoppingCartService shoppingCartService, IOrderService orderService, PaypalService paypalService)
        {
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
            _paypalService = paypalService;
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

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] List<CartDto> cart)
        {
            var result = await _paypalService.CreateOrder(cart);
            return Content(result, "application/json");
        }

        [HttpPost("capture-order/{orderId}")]
        public async Task<IActionResult> CaptureOrder(string orderId)
        {
            var result = await _paypalService.CaptureOrder(orderId);
            return Content(result, "application/json");
        }

    }
}