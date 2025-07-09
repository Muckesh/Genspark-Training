using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using RealEstateApi.Exceptions;
using RealEstateApi.Hubs;
using RealEstateApi.Interfaces;
using RealEstateApi.Mappers;
using RealEstateApi.Misc;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;
using RealEstateApi.Repositories;

namespace RealEstateApi.Services
{
    public class InquiryService : IInquiryService
    {
        InquiryMapper inquiryMapper;
        private readonly IRepository<Guid, Inquiry> _inquiryRepository;
        private readonly IRepository<Guid, InquiryReply> _replyRepository;
        private readonly IRepository<Guid, Agent> _agentRepository;
        private readonly IRepository<Guid, PropertyListing> _listingRepository;
        private readonly IHubContext<InquiryHub> _hubContext;



        private readonly IHttpContextAccessor _httpContextAccessor;


        public InquiryService(
            IRepository<Guid, Inquiry> inquiryRepository,
            IRepository<Guid, InquiryReply> replyRepository,
            IRepository<Guid, PropertyListing> listingRepository,
            IRepository<Guid, Agent> agentRepository,
            IHubContext<InquiryHub> hubContext,
            IHttpContextAccessor httpContextAccessor)
        {
            inquiryMapper = new();
            _inquiryRepository = inquiryRepository;
            _replyRepository = replyRepository;
            _listingRepository = listingRepository;
            _agentRepository = agentRepository;
            _hubContext = hubContext;
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

            // Get the listing to access its AgentId
            var listing = await _listingRepository.GetByIdAsync(inquiryDto.ListingId);
            if (listing == null)
            {
                throw new NotFoundException("Listing not found");
            }
            var inquiry = inquiryMapper.MapInquiryAddRequest(inquiryDto);

            inquiry.AgentId = listing.AgentId;

            inquiry = await _inquiryRepository.AddAsync(inquiry);
            return inquiry ?? throw new FailedOperationException("Unable to create inquiry at the moment.");
        }

        public async Task<PagedResult<InquiryResponseNewDto>> GetFilteredInquiriesAsync(InquiryQueryDto query)
        {
            var inquiries = await _inquiryRepository.GetAllAsync();

            // Filter by ListingId
            if (query.ListingId.HasValue)
                inquiries = inquiries.Where(i => i.ListingId == query.ListingId.Value);

            // Filter by BuyerId
            if (query.BuyerId.HasValue)
                inquiries = inquiries.Where(i => i.BuyerId == query.BuyerId.Value);

            // Filter by AgentId
            if (query.AgentId.HasValue)
                inquiries = inquiries.Where(i => i.AgentId == query.AgentId.Value);

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
            var inquiryDtos = new List<InquiryResponseNewDto>();

            foreach (var inquiry in inquiries)
            {
                IEnumerable<InquiryReply> replies = await GetRepliesAsync(inquiry.Id);
                inquiryDtos.Add(new InquiryResponseNewDto
                {
                    Id = inquiry.Id,
                    ListingId = inquiry.ListingId,
                    ListingTitle = inquiry.Listing?.Title??string.Empty,
                    // Listing = new PropertyListing
                    // {
                    //     Id = inquiry.Listing.Id,
                    //     Title = inquiry.Listing.Title
                    // },
                    BuyerId = inquiry.BuyerId,
                    // Buyer=inquiry.Buyer,
                    // Buyer = new Buyer
                    // {
                    //     Id = inquiry.Buyer.Id,
                    // },
                    Replies=inquiry.Replies??new List<InquiryReply>(),
                    BuyerName = inquiry.Buyer?.User?.Name?? string.Empty,
                    AgentId=inquiry.AgentId,
                    AgentName=inquiry.Agent?.User?.Name ?? string.Empty,
                    Message = inquiry.Message,
                    CreatedAt = inquiry.CreatedAt
                });
            }

            return new PagedResult<InquiryResponseNewDto>
            {
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                Items = inquiryDtos
            };
        }

        public async Task<InquiryReply> AddReplyAsync(Guid inquiryId, AddReplyDto replyDto)
        {
            var inquiry = await _inquiryRepository.GetByIdAsync(inquiryId);
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

            if (!userId.HasValue || string.IsNullOrEmpty(userRole))
                throw new UnauthorizedAccessAppException("Unauthorized access.");

            var reply = new InquiryReply
            {
                InquiryId = inquiryId,
                AuthorId = userId.Value,
                AuthorType = userRole,
                Message = replyDto.Message,
                CreatedAt = DateTime.UtcNow

            };

            // Update inquiry status and agent if needed
            if (userRole == "Agent")
            {
                inquiry.AgentId = userId.Value;
                inquiry.Status = "Replied";
                await _inquiryRepository.UpdateAsync(inquiryId, inquiry);
            }
            else if (userRole == "Buyer")
            {
                inquiry.Status = "Replied";
                await _inquiryRepository.UpdateAsync(inquiryId, inquiry);
            }

            reply = await _replyRepository.AddAsync(reply);

            // Notify all participants in the inquiry
            await _hubContext.Clients.Group($"inquiry-{inquiryId}")
                .SendAsync("ReceiveReply", new
                {
                    // InquiryId = inquiryId,
                    // Reply = reply,
                    // SenderId = userId.Value,
                    // SenderType = userRole
                    InquiryId = inquiryId,
                    Message = reply.Message,
                    AuthorType = userRole,
                    AuthorId = userId.Value,
                    CreatedAt = reply.CreatedAt,


                });

            return reply;


        }

        public async Task<Inquiry?> GetExistingInquiryAsync(Guid listingId, Guid buyerId)
        {
            var inquiries = await _inquiryRepository.GetAllAsync();
            return inquiries.FirstOrDefault(i => i.ListingId == listingId && i.BuyerId == buyerId);
        }


        public async Task<IEnumerable<InquiryReply>> GetRepliesAsync(Guid inquiryId)
        {
            return await ((InquiryReplyRepository)_replyRepository).GetRepliesForInquiryAsync(inquiryId);
        }
        
        public async Task<InquiryWithRepliesDto> GetInquiryWithRepliesAsync(Guid inquiryId)
        {
            var inquiry = await _inquiryRepository.GetByIdAsync(inquiryId);
            var replies = await GetRepliesAsync(inquiryId);

            return new InquiryWithRepliesDto
            {
                Inquiry = inquiry,
                Replies = replies.ToList()
            };
        }

    }
}