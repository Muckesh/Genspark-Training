using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IInquiryService
    {
        Task<Inquiry> CreateInquiryAsync(AddInquiryDto inquiryDto);
        Task<PagedResult<InquiryResponseNewDto>> GetFilteredInquiriesAsync(InquiryQueryDto query);

        Task<InquiryReply> AddReplyAsync(Guid inquiryId, AddReplyDto replyDto);
        Task<IEnumerable<InquiryReply>> GetRepliesAsync(Guid inquiryId);
        Task<InquiryWithRepliesDto> GetInquiryWithRepliesAsync(Guid inquiryId);

        Task<Inquiry?> GetExistingInquiryAsync(Guid listingId, Guid buyerId);

    }
}