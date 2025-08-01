namespace Ecommerce.Models.DTOs
{
    public class ProductQueryParamsDto
    {
        public string? ProductName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? ColorName { get; set; }
        public string? ModelName { get; set; }
    }
}