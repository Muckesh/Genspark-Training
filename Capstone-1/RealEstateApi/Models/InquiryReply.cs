namespace RealEstateApi.Models
{
    public class InquiryReply
    {
        public Guid Id { get; set; }
        public Guid InquiryId { get; set; }
        public Inquiry? Inquiry { get; set; }

        public Guid AuthorId { get; set; } // Could be AgentId or BuyerId
        public string AuthorType { get; set; } = string.Empty; // "Agent" or "Buyer"


        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

// public class AddInquiryReplyDto
// {
//     public Guid InquiryId { get; set; }
//     public string Message { get; set; } = string.Empty;
// }


// public class InquiryReplyDto
// {
//     public Guid Id { get; set; }
//     public Guid InquiryId { get; set; }
//     public Guid AgentId { get; set; }
//     public string Message { get; set; }
//     public DateTime RepliedAt { get; set; }
// }


// public interface IInquiryReplyService
// {
//     Task<InquiryReplyDto> AddReplyAsync(AddInquiryReplyDto replyDto);
//     Task<IEnumerable<InquiryReplyDto>> GetRepliesByInquiryIdAsync(Guid inquiryId);
// }

// public class InquiryReplyService : IInquiryReplyService
// {
//     private readonly IRepository<Guid, InquiryReply> _replyRepository;
//     private readonly IRepository<Guid, Inquiry> _inquiryRepository;
//     private readonly IHttpContextAccessor _httpContextAccessor;

//     public InquiryReplyService(
//         IRepository<Guid, InquiryReply> replyRepository,
//         IRepository<Guid, Inquiry> inquiryRepository,
//         IHttpContextAccessor httpContextAccessor)
//     {
//         _replyRepository = replyRepository;
//         _inquiryRepository = inquiryRepository;
//         _httpContextAccessor = httpContextAccessor;
//     }

//     public async Task<InquiryReplyDto> AddReplyAsync(AddInquiryReplyDto replyDto)
//     {
//         var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
//         var userRole = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;

//         if (userRole != "Agent" || !userId.HasValue)
//             throw new UnauthorizedAccessAppException("Only agents can reply to inquiries.");

//         var inquiry = await _inquiryRepository.GetByIdAsync(replyDto.InquiryId);
//         if (inquiry == null)
//             throw new NotFoundAppException("Inquiry not found.");

//         var reply = new InquiryReply
//         {
//             Id = Guid.NewGuid(),
//             InquiryId = inquiry.Id,
//             AgentId = userId.Value,
//             Message = replyDto.Message,
//             RepliedAt = DateTime.UtcNow
//         };

//         await _replyRepository.AddAsync(reply);

//         return new InquiryReplyDto
//         {
//             Id = reply.Id,
//             InquiryId = reply.InquiryId,
//             AgentId = reply.AgentId,
//             Message = reply.Message,
//             RepliedAt = reply.RepliedAt
//         };
//     }

//     public async Task<IEnumerable<InquiryReplyDto>> GetRepliesByInquiryIdAsync(Guid inquiryId)
//     {
//         var replies = await _replyRepository.GetAllAsync();
//         var result = replies
//             .Where(r => r.InquiryId == inquiryId)
//             .Select(r => new InquiryReplyDto
//             {
//                 Id = r.Id,
//                 InquiryId = r.InquiryId,
//                 AgentId = r.AgentId,
//                 Message = r.Message,
//                 RepliedAt = r.RepliedAt
//             });

//         return result;
//     }
// }

// [ApiController]
// [ApiVersion("1.0")]
// [Route("api/v{version:apiVersion}/[controller]")]
// public class InquiryRepliesController : ControllerBase
// {
//     private readonly IInquiryReplyService _inquiryReplyService;

//     public InquiryRepliesController(IInquiryReplyService inquiryReplyService)
//     {
//         _inquiryReplyService = inquiryReplyService;
//     }

//     [HttpPost]
//     [Authorize(Roles = "Agent")]
//     public async Task<IActionResult> AddReply(AddInquiryReplyDto replyDto)
//     {
//         try
//         {
//             var result = await _inquiryReplyService.AddReplyAsync(replyDto);
//             return CreatedAtAction(nameof(GetReplies), new { inquiryId = result.InquiryId }, result);
//         }
//         catch (UnauthorizedAccessAppException e)
//         {
//             return Forbid(e.Message);
//         }
//         catch (NotFoundAppException e)
//         {
//             return NotFound(e.Message);
//         }
//         catch (Exception e)
//         {
//             return BadRequest(e.Message);
//         }
//     }

//     [HttpGet("{inquiryId}")]
//     public async Task<IActionResult> GetReplies(Guid inquiryId)
//     {
//         var replies = await _inquiryReplyService.GetRepliesByInquiryIdAsync(inquiryId);
//         return Ok(replies);
//     }
// }

// public class InquiryResponseDto
// {
//     public Guid Id { get; set; }
//     public Guid ListingId { get; set; }
//     public PropertyListing? Listing { get; set; }
//     public Guid BuyerId { get; set; }
//     public Buyer? Buyer { get; set; }
//     public string Message { get; set; } = string.Empty;
//     public DateTime CreatedAt { get; set; }

//     public List<InquiryReplyDto> Replies { get; set; } = new();
// }
