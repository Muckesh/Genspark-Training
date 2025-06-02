using BankApi.Interfaces;
using BankApi.Models;
using BankApi.Models.DTOs;

namespace BankApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<Guid, Account> _accountRepository;
        public AccountService(IRepository<Guid, Account> accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<Account> CreateAccount(AccountAddDto accountAddDto)
        {
            try
            {
                var newAccount = new Account
                {
                    Name = accountAddDto.Name,
                    Balance = accountAddDto.Balance
                };
                newAccount = await _accountRepository.Add(newAccount);
                return newAccount;
            }
            catch (Exception e)
            {
                throw new Exception("Error adding account : "+e.Message);
            }
        }

        public async Task<Account> DeleteAccount(Guid accountId)
        {
            try
            {
                var account = await _accountRepository.Get(accountId);
                account.Status = "Inactive";
                account = await _accountRepository.Update(accountId,account);
                return account;
            }
            catch (Exception e)
            {
                throw new Exception("Error deleting the account."+e.Message);
            }
        }

        public async Task<Account> GetAccountById(Guid accountId)
        {
            try
            {
                var account = await _accountRepository.Get(accountId);
                if (account == null)
                {
                    throw new Exception("Account not found with the given ID.");
                }
                return account;
            }
            catch (Exception e)
            {
                throw new Exception("Error finding the account."+e.Message);
            }
        }

        public async Task<IEnumerable<Account>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountRepository.GetAll();
                if (accounts.Count() == 0)
                {
                    throw new Exception("No accounts in the database.");
                }
                return accounts;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}