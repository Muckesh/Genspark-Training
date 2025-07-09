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

        public string? PropertyType { get; set; } // flat, apartments
        public string? ListingType { get; set; } //rent,buy
        public bool? IsPetsAllowed { get; set; }
        public string? status { get; set; } // sold, available
        public bool? HasParking { get; set; } 
    }

}