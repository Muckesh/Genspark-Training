using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class InquiryRepository : Repository<Guid, Inquiry>
    {
        public InquiryRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }
        public override async Task<IEnumerable<Inquiry>> GetAllAsync()
        {
            var inquiries = await _realEstateDbContext.Inquiries
                                    .Include(i => i.Buyer)
                                    .Include(i => i.Listing)
                                    .ToListAsync();

            // return inquiries.Count == 0 ? throw new Exception("No inquiries found") : inquiries;
            return inquiries;

        }

        public override async Task<Inquiry> GetByIdAsync(Guid id)
        {
            var inquiry = await _realEstateDbContext.Inquiries
                                .Include(i => i.Buyer)
                                .Include(i => i.Listing)
                                .SingleOrDefaultAsync(i => i.Id == id);
            return inquiry ?? throw new NotFoundException("Inquiry not found.");
        }
    }
}