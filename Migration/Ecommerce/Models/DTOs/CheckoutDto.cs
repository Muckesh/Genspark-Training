namespace Ecommerce.Models.DTOs
{
    public class CheckoutDto
    {
        public string OrderName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public List<CartDto> Items { get; set; } = new();
        public string PaymentType { get; set; } = "Cash"; // or "PayPal"
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}