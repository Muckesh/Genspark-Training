namespace RealEstateApi.Models.DTOs
{
    public class PropertyListingResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int SquareFeet { get; set; }
        public string PropertyType { get; set; } = string.Empty;
        public string ListingType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsPetsAllowed { get; set; }
        public bool HasParking { get; set; }
        public Guid AgentId { get; set; }
        // public Agent Agent { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}