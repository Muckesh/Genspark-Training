namespace RealEstateApi.Models.DTOs
{
    public class InquiryQueryDto
    {
        public Guid? ListingId { get; set; }
        public Guid? BuyerId { get; set; }

        public Guid? AgentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string? Keyword { get; set; }

        public string? SortBy { get; set; } = "createdDate";
        public bool IsDescending { get; set; } = true;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}