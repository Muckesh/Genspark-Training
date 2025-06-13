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

        public Guid? AgentId { get; set; }


    }
}