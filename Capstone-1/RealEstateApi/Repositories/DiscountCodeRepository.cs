using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class DiscountCodeRepository : Repository<Guid, DiscountCodes>
    {
        public DiscountCodeRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }
        public override async Task<IEnumerable<DiscountCodes>> GetAllAsync()
        {
            var codes = await _realEstateDbContext.DiscountCodes
                                    .Where(p=>p.IsDeleted==false)
                                    .ToListAsync();

            return codes;

        }

        public override async Task<DiscountCodes> GetByIdAsync(Guid id)
        {
            var code = await _realEstateDbContext.DiscountCodes
                                
                                .SingleOrDefaultAsync(i => i.IsDeleted == false  && i.Id == id);
            return code ?? throw new NotFoundException("code not found.");
        }
    }
}