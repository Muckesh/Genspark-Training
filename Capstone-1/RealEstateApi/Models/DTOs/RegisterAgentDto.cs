namespace RealEstateApi.Models.DTOs
{
    public class RegisterAgentDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

    }
}