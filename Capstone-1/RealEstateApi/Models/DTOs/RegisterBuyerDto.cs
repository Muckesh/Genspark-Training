namespace RealEstateApi.Models.DTOs
{
    public class RegisterBuyerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        public string PreferredLocation { get; set; } = string.Empty;
        public double Budget { get; set; }
    }
}