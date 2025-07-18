using RealEstateApi.Exceptions;
using RealEstateApi.Interfaces;
using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly IRepository<Guid, DiscountCodes> _discountcoderepo;
        public DiscountCodeService(IRepository<Guid, DiscountCodes> discountcoderepo)
        {
            _discountcoderepo = discountcoderepo;
        }

        public async Task<DiscountCodes> AddDiscountCode(AddDiscountCodeDto codes)
        {
            var allCodes = await _discountcoderepo.GetAllAsync();
            bool codeExists = allCodes.Any(e => e.Code == codes.Code);

            if (codeExists)
            {
                throw new Exception("Code already exist");
            }

            var NewCode = new DiscountCodes
            {
                Id = new Guid(),
                Code = codes.Code,
                DiscountPercentage = codes.DiscountPercentage,
                Remaining = codes.Remaining,
                ExpiryDate = codes.ExpiryDate,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                // DeletedAt=DateTime.MinValue,
                Status = "Available"
            };

            await _discountcoderepo.AddAsync(NewCode);
            return NewCode;
        }

        public async Task<DiscountCodes> CheckCodeAvail(string code)
        {
            var allcodes = await _discountcoderepo.GetAllAsync();
            bool codeExists = allcodes.Any(e => e.Code == code && e.IsDeleted == false && e.Remaining > 0 && e.Status == "Available" && e.ExpiryDate > DateTime.UtcNow);
            if (codeExists)
            {
                var DiscountCode = allcodes.Where(e => e.Code == code).FirstOrDefault();

                return DiscountCode;
            }
            else
            {
                throw new NotFoundException("Invalid Code");
            }
        }

        public async Task<IEnumerable<DiscountCodes>> GetCodes()
        {
            var allCodes = await _discountcoderepo.GetAllAsync();
            foreach (var code in allCodes)
            {
                if (code.ExpiryDate < DateTime.UtcNow && code.Status != "Expired")
                {
                    code.Status = "Expired";
                    await _discountcoderepo.UpdateAsync(code.Id,code);
                }
            }
            return allCodes;
        }

        public async Task<bool> SoftDeleteCode(Guid id)
        {

            var code = await _discountcoderepo.GetByIdAsync(id);
            if (code == null) throw new NotFoundException("code not found");

            code.IsDeleted = true;
            code.DeletedAt = DateTime.UtcNow;
            code.Remaining = 0;
            code.Status = "Deleted";

            var updated = await _discountcoderepo.UpdateAsync(code.Id, code);
            return updated != null;
        }

        public async Task<DiscountCodes> PatchCodeExpire(Guid id)
        {
            var code = await _discountcoderepo.GetByIdAsync(id);
            if (code == null) throw new NotFoundException("code not found");

            code.Status = "Expired";
            var updated = await _discountcoderepo.UpdateAsync(code.Id, code);
            return updated;
        }
    }
}
