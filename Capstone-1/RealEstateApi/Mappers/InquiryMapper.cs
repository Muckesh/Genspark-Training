using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Mappers
{
    public class InquiryMapper
    {
        public Inquiry MapInquiryAddRequest(AddInquiryDto inquiryDto)
        {
            Inquiry inquiry = new();
            inquiry.ListingId = inquiryDto.ListingId;
            inquiry.BuyerId = inquiryDto.BuyerId.Value;
            // inquiry.AgentId = inquiryDto.ListingId;
            inquiry.Message = inquiryDto.Message;

            return inquiry;
        }
    }
}