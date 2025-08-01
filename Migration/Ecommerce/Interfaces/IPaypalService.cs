using Ecommerce.Models.DTOs;

namespace Ecommerce.Interfaces
{
    public interface IPaypalService
    {
        Task<string> CreatePaypalOrderAsync(List<CartDto> cartItems, string currency = "USD");
        Task<string> CapturePaypalOrderAsync(string orderId);
        // Task<OrderResponseDto> CapturePaypalOrderAsync(string orderId);
        // Task<string> CreatePaypalPaymentAsync(CheckoutDto dto);

        // Task<OrderResponseDto> ExecutePaypalPayment(string paymentId, string payerId, CheckoutDto dto);

    }
}