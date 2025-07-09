namespace RealEstateApi.Models
{
    public class Inquiry
    {
        public Guid Id { get; set; }

        public Guid ListingId { get; set; }
        public PropertyListing? Listing { get; set; }

        public Guid BuyerId { get; set; }
        public Buyer? Buyer { get; set; }

        // new
        public Guid? AgentId { get; set; }
        public Agent? Agent { get; set; }

        // Inquiry status
        public string Status { get; set; } = "Pending";
        
        // Replies
        public ICollection<InquiryReply>? Replies { get; set; }


        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}