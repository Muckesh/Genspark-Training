using RealEstateApi.Models;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Interfaces
{
    public interface IDiscountCodeService
    {
        // Task<DiscountCodes> AddDiscountCode(DiscountCodes codes);
        public Task<DiscountCodes> AddDiscountCode(AddDiscountCodeDto codes);

        Task<IEnumerable<DiscountCodes>> GetCodes();

        public Task<bool> SoftDeleteCode(Guid id);

        public Task<DiscountCodes> CheckCodeAvail(string code);

        public  Task<DiscountCodes> PatchCodeExpire(Guid id);
    }
}