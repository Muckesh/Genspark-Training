namespace Ecommerce.Models.DTOs
{
    public class OrderDetailResponseDto
    {
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}