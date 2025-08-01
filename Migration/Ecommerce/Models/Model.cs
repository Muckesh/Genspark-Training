using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class Model
    {
        [Key]
        public int ModelId { get; set; }
        public string ModelName { get; set; } = string.Empty;
    
        public ICollection<Product>? Products { get; set; }
    }
}