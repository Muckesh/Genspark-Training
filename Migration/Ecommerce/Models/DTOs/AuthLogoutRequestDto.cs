namespace Ecommerce.Models.DTOs
{
    public class AuthLogoutRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}