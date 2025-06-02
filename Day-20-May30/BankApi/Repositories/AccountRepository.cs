using BankApi.Contexts;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories
{
    public class AccountRepository : Repository<Guid, Account>
    {
        public AccountRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
            
        }
        public override async Task<Account> Get(Guid key)
        {
            var account = await _bankDbContext.Accounts.SingleOrDefaultAsync(a => a.Id == key);
            if (account == null)
            {
                throw new Exception("Account not found with the given ID.");
            }
            return account;
        }

        public override async Task<IEnumerable<Account>> GetAll()
        {
            var accounts = _bankDbContext.Accounts;
            if (accounts.Count() == 0)
            {
                throw new Exception("No accounts in the database.");
            }
            return (await accounts.ToListAsync());
        }
    }
}