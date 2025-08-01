using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Color
    {
        [Key]
        public int ColorId { get; set; }
        public string ColorName { get; set; } = string.Empty;
    
        public ICollection<Product>? Products { get; set; }
    }
}