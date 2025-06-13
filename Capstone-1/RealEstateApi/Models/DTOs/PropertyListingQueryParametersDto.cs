namespace RealEstateApi.Models.DTOs
{
    public class PropertyListingQueryParametersDto
    {
        // Search
        public string? Keyword { get; set; }

        // Filter
        public string? Location { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public int? MinBedrooms { get; set; }
        public int? MinBathrooms { get; set; }

        // Sorting
        public string? SortBy { get; set; } = "Price"; // default
        public bool IsDescending { get; set; } = false;

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}