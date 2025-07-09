namespace RealEstateApi.Models.DTOs
{
    public class AddInquiryDto
    {
        public Guid ListingId { get; set; }

        public Guid? BuyerId { get; set; }

        // public Guid? AgentId { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}