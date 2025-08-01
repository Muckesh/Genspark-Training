namespace Ecommerce.Models.DTOs
{
    public class OrderDetailRequestDto
    {
        public int ProductID { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}