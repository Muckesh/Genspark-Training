using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.Models.DTOs
{
    public class AddReplyDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Message { get; set; } = string.Empty;
    }

    public class InquiryWithRepliesDto
    {
        public Inquiry Inquiry { get; set; }
        public List<InquiryReply> Replies { get; set; }
    }
   
}