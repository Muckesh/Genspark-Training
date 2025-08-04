using Microsoft.EntityFrameworkCore;
using RealEstateApi.Contexts;
using RealEstateApi.Exceptions;
using RealEstateApi.Models;

namespace RealEstateApi.Repositories
{
    public class PasswordResetTokenRepository : Repository<Guid, PasswordResetToken>
    {
        public PasswordResetTokenRepository(RealEstateDbContext realEstateDbContext) : base(realEstateDbContext)
        {
            
        }
        public override async Task<IEnumerable<PasswordResetToken>> GetAllAsync()
        {
            var tokens = await _realEstateDbContext
                                    .PasswordResetTokens
                                    .Include(t=>t.User)
                                    .ToListAsync();
            return tokens;
        }

        public override async Task<PasswordResetToken> GetByIdAsync(Guid id)
        {
            var token = await _realEstateDbContext
                                    .PasswordResetTokens
                                    .Include(t=>t.User)
                                    .SingleOrDefaultAsync(t => t.Id == id);
            return token ?? throw new NotFoundException("Token not found");
        }
    }
}