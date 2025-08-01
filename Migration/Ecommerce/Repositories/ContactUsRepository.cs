using Ecommerce.Contexts;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class ContactUsRepository : Repository<int, ContactUs>
    {
        public ContactUsRepository(EcommerceDbContext ecommerceDbContext) : base(ecommerceDbContext)
        {

        }

        public override async Task<ICollection<ContactUs>> GetAllAsync()
        {
            var contactUsList = await _ecommerceDbContext.ContactUs.ToListAsync();
            return contactUsList;
        }

        public override async Task<ContactUs> GetByIdAsync(int key)
        {
            var contactUs = await _ecommerceDbContext.ContactUs.SingleOrDefaultAsync(cu => cu.Id == key);
            return contactUs ?? throw new KeyNotFoundException("Contact us not found.");
        }
    }
}