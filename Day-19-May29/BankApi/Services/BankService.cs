// using BankApi.Interfaces;
// using BankApi.Models;
// using BankApi.Models.DTOs;

// namespace BankApi.Services
// {
//     public class BankService : IBankService
//     {
//         private readonly IRepository<Guid, Account> _accountRepository;
//         private readonly IRepository<Guid, Transaction> _transactionRepository;
//         public BankService(IRepository<Guid, Account> accountRepository, IRepository<Guid, Transaction> transactionRepository)
//         {
//             _accountRepository = accountRepository;
//             _transactionRepository = transactionRepository;
//         }

//         public async Task<Account> CreateAccount(AccountAddDto account)
//         {
//             try
//             {
//                 var newAccount = new Account
//                 {
//                     Name = account.Name,
//                     Balance = account.Balance,
//                     CreatedAt = DateTime.UtcNow
//                 };
//                 var result = await _accountRepository.Add(newAccount);
//                 return result;
//             }
//             catch (Exception e)
//             {

//                 throw new Exception("Error adding account : " + e.Message);
//             }
//         }

//         public async Task<Transaction> Deposit(DepositDto depositDto)
//         {
//             var account = await _accountRepository.Get(depositDto.AccountId);
//             if (account == null)
//                 throw new Exception("Account not found");
//             account.Balance += depositDto.Amount;
//             var transaction = new Transaction
//             {
//                 AccountId = depositDto.AccountId,
//                 Amount = depositDto.Amount,
//                 Type = TransactionType.Deposit,
//                 Timestamp = DateTime.UtcNow
//             };
//             transaction = await _transactionRepository.Add(transaction);
//             return transaction;
//         }

//         public async Task<IEnumerable<Account>> GetAllAccounts()
//         {
//             var accounts = await _accountRepository.GetAll();
//             return accounts;
//         }

//         public async Task<IEnumerable<Transaction>> GetAllTransactions()
//         {
//             var transactions = await _transactionRepository.GetAll();
//             return transactions;
//         }

//         public async Task<Transaction> Withdraw(WithdrawDto withdrawDto)
//         {
//             var account = await _accountRepository.Get(withdrawDto.AccountId);
//             if (account == null)
//                 throw new Exception("Account not found");
//             account.Balance -= withdrawDto.Amount;
//             var transaction = new Transaction
//             {
//                 AccountId = withdrawDto.AccountId,
//                 Amount = withdrawDto.Amount,
//                 Type = TransactionType.Withdrawal,
//                 Timestamp = DateTime.UtcNow
//             };
//             transaction = await _transactionRepository.Add(transaction);
//             return transaction;
//         }
//     }
// }