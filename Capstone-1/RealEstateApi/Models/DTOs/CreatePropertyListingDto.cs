namespace RealEstateApi.Models.DTOs
{
    public class CreatePropertyListingDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int SquareFeet { get; set; }

        public string PropertyType { get; set; } = string.Empty; // flat, apartments
        public string ListingType { get; set; } = string.Empty;//rent,buy
        public bool IsPetsAllowed { get; set; } = false;
        public string Status { get; set; } = string.Empty;// sold, available
        public bool HasParking { get; set; } = false;


        public Guid? AgentId { get; set; }


    }
}