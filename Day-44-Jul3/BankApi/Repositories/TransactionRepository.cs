using BankApi.Models;
using BankApi.Contexts;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Repositories
{
    public class TransactionRepository : Repository<Guid, Transaction>
    {
        public TransactionRepository(BankDbContext bankDbContext) : base(bankDbContext)
        {
            
        }
        public override async Task<Transaction> Get(Guid key)
        {
            var transaction = await _bankDbContext.Transactions.SingleOrDefaultAsync(t => t.Id == key);
            if (transaction == null)
            {
                throw new Exception("Transaction not found with the given ID.");
            }
            return transaction;
        }

        public override async Task<IEnumerable<Transaction>> GetAll()
        {
            var transactions = _bankDbContext.Transactions;
            if (transactions.Count() == 0)
            {
                throw new Exception("No transactions in the database.");
            }
            return (await transactions.ToListAsync());
        }
    }
}