namespace RealEstateApi.Models.DTOs
{
    public class UpdatePropertyListingDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? Location { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public int? SquareFeet { get; set; }
    }

}