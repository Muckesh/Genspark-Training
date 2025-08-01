namespace Ecommerce.Models.DTOs
{
    public class OrderResponseDto
    {
        public int OrderID { get; set; }
        // public string? PayPalOrderId { get; set; }

        public string OrderName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new();
    }
}