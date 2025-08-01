using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        

        public ICollection<News>? News { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}