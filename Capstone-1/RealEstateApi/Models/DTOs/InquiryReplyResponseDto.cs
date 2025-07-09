namespace RealEstateApi.Models.DTOs
{
    public class InquiryReplyResponseDto
    {
        public Guid Id { get; set; }
        public Guid InquiryId { get; set; }

        public Guid AuthorId { get; set; } // Could be AgentId or BuyerId
        public string AuthorType { get; set; } = string.Empty; // "Agent" or "Buyer"


        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}