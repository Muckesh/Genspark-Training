namespace Ecommerce.Models.DTOs
{
    public class ContactRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string RecaptchaToken { get; set; } = string.Empty;
    }
}