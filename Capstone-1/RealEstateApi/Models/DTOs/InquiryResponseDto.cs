namespace RealEstateApi.Models.DTOs
{
    public class InquiryResponseDto
    {
        public Guid Id { get; set; }

        public Guid ListingId { get; set; }
        public PropertyListing? Listing { get; set; }

        public Guid BuyerId { get; set; }
        public Buyer? Buyer { get; set; }

        // new
        public Guid AgentId { get; set; }
        public Agent? Agent { get; set; }

        // Inquiry status
        public string Status { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

    }
    
     public class InquiryResponseNewDto
    {
        public Guid Id { get; set; }

        public Guid ListingId { get; set; }
        public string ListingTitle { get; set; } = string.Empty;

        public Guid? BuyerId { get; set; }
        public string BuyerName { get; set; }=string.Empty;

        // new
        public Guid? AgentId { get; set; }
        public string AgentName { get; set; } = string.Empty;

        // Inquiry status
        public string Status { get; set; } = string.Empty;

        // public In MyProperty { get; set; }
        public IEnumerable<InquiryReply> Replies { get; set; } = [];
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    
    }
}