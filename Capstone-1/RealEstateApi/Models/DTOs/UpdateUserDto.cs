namespace RealEstateApi.Models.DTOs
{
    public class UpdateUserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; } // Optional — validate uniqueness
        public string? Role { get; set; }  // Optional
    }

}