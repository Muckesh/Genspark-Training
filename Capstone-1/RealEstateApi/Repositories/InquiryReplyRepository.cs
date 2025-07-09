using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class InquiryReplyRepository : Repository<Guid, InquiryReply>
    {
        public InquiryReplyRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {

        }
        public override async Task<IEnumerable<InquiryReply>> GetAllAsync()
        {
            return await _realEstateDbContext.InquiryReplies
                        .Include(r => r.Inquiry)
                        .ToListAsync();
        }

        public override async Task<InquiryReply> GetByIdAsync(Guid id)
        {
            return await _realEstateDbContext.InquiryReplies
                            .Include(r => r.Inquiry)
                            .FirstOrDefaultAsync(r => r.Id == id)
                            ?? throw new NotFoundException("Reply Not found");
        }
        
        public async Task<IEnumerable<InquiryReply>> GetRepliesForInquiryAsync(Guid inquiryId)
        {
            return await _realEstateDbContext.InquiryReplies
                .Where(r => r.InquiryId == inquiryId)
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();
        }

    }
}