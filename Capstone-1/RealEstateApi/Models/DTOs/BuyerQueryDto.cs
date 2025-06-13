namespace RealEstateApi.Models.DTOs
{
    public class BuyerQueryDto
    {
        public string? PreferredLocation { get; set; }
        public double? MinBudget { get; set; }
        public double? MaxBudget { get; set; }

        public string? SortBy { get; set; } // e.g., "budget", "location", "name"
        public bool IsDescending { get; set; } = false;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}