using BankApi.Models;
using BankApi.Contexts;
using BankApi.Interfaces;
using BankApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BankApi.Services
{
    public class TransferServiceWithTransaction : ITransferServiceWithTransaction
    {
        private readonly BankDbContext _bankDbContext;

        public TransferServiceWithTransaction(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }
        public async Task<IEnumerable<Transaction>> Transfer(TransferDto transferDto)
        {
            var sourceAccount = await _bankDbContext.Accounts.SingleOrDefaultAsync(sa => sa.Id == transferDto.SourceAccountId);
            var targetAccount = await _bankDbContext.Accounts.SingleOrDefaultAsync(ta => ta.Id == transferDto.TargetAccountId);

            if (sourceAccount == null || targetAccount == null || sourceAccount.Status == "Inactive" || targetAccount.Status == "Inactive")
            {
                throw new Exception("Account not found.");
            }

            var transactions = await transferMoney(sourceAccount,targetAccount,transferDto.Amount);
            return transactions;
        }

        private async Task<IEnumerable<Transaction>> transferMoney(Account sourceAccount, Account targetAccount, decimal amount)
        {
            List<Transaction> transactions = new();
            using var transaction = _bankDbContext.Database.BeginTransaction();
            try
            {

                if (sourceAccount.Balance < amount)
                {
                    throw new Exception("Balance not enough.");
                }
                sourceAccount.Balance -= amount;
                targetAccount.Balance += amount;
                var sourceTransaction = new Transaction
                {
                    AccountId = sourceAccount.Id,
                    TargetAccountId = targetAccount.Id,
                    Amount = amount*-1,
                    Type = TransactionType.Transfer
                };

                await _bankDbContext.Transactions.AddAsync(sourceTransaction);
                await _bankDbContext.SaveChangesAsync();
                transactions.Add(sourceTransaction);

                var targetTransaction = new Transaction
                {
                    AccountId = targetAccount.Id,
                    TargetAccountId = sourceAccount.Id,
                    Amount = amount,
                    Type = TransactionType.Transfer
                };

                await _bankDbContext.Transactions.AddAsync(targetTransaction);
                await _bankDbContext.SaveChangesAsync();
                transactions.Add(targetTransaction);

                await transaction.CommitAsync();
                return transactions;
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();        
                throw new Exception(e.Message);
            }

        }
    }
}