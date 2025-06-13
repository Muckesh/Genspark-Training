
namespace RealEstateApi.Models
{
    public class PropertyListing
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int SquareFeet { get; set; }

        public Guid AgentId { get; set; }
        public Agent? Agent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;

        // Nav
        public ICollection<PropertyImage>? Images { get; set; }
        public ICollection<Inquiry>? Inquiries { get; set; }
    }
}