namespace RealEstateApi.Models.DTOs
{
    public class PropertyListingQueryParametersDto
    {
        // id
        public Guid? ListingId { get; set; }

        // Search
        public string? Keyword { get; set; }

        public Guid? AgentId { get; set; }

        // Filter
        public string? Location { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MinBathrooms { get; set; }
        public string? PropertyType { get; set; }
        public string? ListingType { get; set; }
        public string? Status { get; set; }
        public bool? IsPetsAllowed { get; set; }
        public bool? HasParking { get; set; }

        // Sorting
        public string? SortBy { get; set; } = "CreatedAt"; // default
        public bool IsDescending { get; set; } = true;

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}