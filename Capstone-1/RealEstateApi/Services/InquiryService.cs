using System.Security.Claims;
using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Mappers;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class InquiryService : IInquiryService
    {
        InquiryMapper inquiryMapper;
        private readonly IRepository<Guid, Inquiry> _inquiryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public InquiryService(IRepository<Guid, Inquiry> inquiryRepository,IHttpContextAccessor httpContextAccessor)
        {
            inquiryMapper = new();
            _inquiryRepository = inquiryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Inquiry> CreateInquiryAsync(AddInquiryDto inquiryDto)
        {
           

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            if (!userId.HasValue || string.IsNullOrEmpty(userRole))
                throw new UnauthorizedAccessAppException("Unauthorized access.");

            if (userRole == "Buyer")
            {
                // forcing buyers own ID, ignore anything passed in the DTO
                inquiryDto.BuyerId = userId.Value;
            }
            else if (userRole == "Admin")
            {
                if (inquiryDto.BuyerId.HasValue)
                    throw new ArgumentRequiredException("BuyerId is required when admin creates an inquiry.");
            }
            else
            {
                throw new UnauthorizedAccessAppException("Only Buyers and Admins can create inquiries.");
            }


            var inquiry = inquiryMapper.MapInquiryAddRequest(inquiryDto);

            inquiry = await _inquiryRepository.AddAsync(inquiry);
            return inquiry ?? throw new FailedOperationException("Unable to create inquiry at the moment.");
        }

        public async Task<PagedResult<Inquiry>> GetFilteredInquiriesAsync(InquiryQueryDto query)
        {
            var inquiries = await _inquiryRepository.GetAllAsync();

            // Filter by ListingId
            if (query.ListingId.HasValue)
                inquiries = inquiries.Where(i => i.ListingId == query.ListingId.Value);

            // Filter by BuyerId
            if (query.BuyerId.HasValue)
                inquiries = inquiries.Where(i => i.BuyerId == query.BuyerId.Value);

            // Filter by CreatedDate range
            if (query.FromDate.HasValue)
                inquiries = inquiries.Where(i => i.CreatedAt >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                inquiries = inquiries.Where(i => i.CreatedAt <= query.ToDate.Value);

            // Keyword Search in Message
            if (!string.IsNullOrWhiteSpace(query.Keyword))
                inquiries = inquiries.Where(i => i.Message.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase));

            // Sorting
            inquiries = query.SortBy?.ToLower() switch
            {
                "createddate" => query.IsDescending ? inquiries.OrderByDescending(i => i.CreatedAt) : inquiries.OrderBy(i => i.CreatedAt),
                _ => inquiries.OrderByDescending(i => i.CreatedAt)
            };

            // Total count before pagination
            int totalCount = inquiries.Count();

            // Pagination
            inquiries = inquiries
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize);

            return new PagedResult<Inquiry>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = inquiries
            };
        }
    }
}