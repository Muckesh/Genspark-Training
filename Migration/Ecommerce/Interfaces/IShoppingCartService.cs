using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IShoppingCartService
    {
        Task<OrderResponseDto> PlaceOrderAsync(CheckoutDto dto);
        // Task<OrderResponseDto> CompletePaypalOrderAsync(string paypalOrderId);
        // Task<OrderResponseDto> PlaceOrderPaypalAsync(CheckoutDto dto);
    }
}