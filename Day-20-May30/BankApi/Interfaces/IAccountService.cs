using BankApi.Models;
using BankApi.Models.DTOs;

namespace BankApi.Interfaces
{
    public interface IAccountService
    {
        public Task<IEnumerable<Account>> GetAllAccounts();
        public Task<Account> GetAccountById(Guid accountId);
        public Task<Account> CreateAccount(AccountAddDto accountAddDto);
        public Task<Account> DeleteAccount(Guid accountId);
        // Get all transactions by accountId
    }
}