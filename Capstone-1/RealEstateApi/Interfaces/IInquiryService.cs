using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IInquiryService
    {
        Task<Inquiry> CreateInquiryAsync(AddInquiryDto inquiryDto);
        Task<PagedResult<Inquiry>> GetFilteredInquiriesAsync(InquiryQueryDto query);

    }
}