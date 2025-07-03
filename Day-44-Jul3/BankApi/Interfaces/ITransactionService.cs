using BankApi.Models;
using BankApi.Models.DTOs;

namespace BankApi.Interfaces
{
    public interface ITransactionService
    {
        public Task<IEnumerable<Transaction>> GetAllTransactions();
        public Task<Transaction> GetTransactionById(Guid transactionId);
        public Task<Transaction> DepositAmount(DepositDto depositDto);
        public Task<Transaction> WithdrawAmount(WithdrawDto withdrawDto);
        public Task<Transaction> TransferAmount(TransferDto transferDto);
        // Get all transactions by accountId

    }
}