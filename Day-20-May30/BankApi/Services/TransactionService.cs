using BankApi.Interfaces;
using BankApi.Models;
using BankApi.Models.DTOs;

namespace BankApi.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Guid, Transaction> _transactionRepository;
        private readonly IRepository<Guid, Account> _accountRepository;

        public TransactionService(IRepository<Guid, Transaction> transactionRepository,IRepository<Guid,Account>accountRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Transaction> DepositAmount(DepositDto depositDto)
        {
            var account = await _accountRepository.Get(depositDto.AccountId);
            if (account == null)
                throw new Exception("Account not found");
            account.Balance += depositDto.Amount;
            var transaction = new Transaction
            {
                AccountId = depositDto.AccountId,
                Amount = depositDto.Amount,
                Type = TransactionType.Deposit,
                Timestamp = DateTime.UtcNow
            };
            transaction = await _transactionRepository.Add(transaction);
            return transaction;
        }

        public async Task<IEnumerable<Transaction>> GetAllTransactions()
        {
            var transactions = await _transactionRepository.GetAll();
            return transactions;
        }

        public async Task<Transaction> GetTransactionById(Guid transactionId)
        {
            var transaction = await _transactionRepository.Get(transactionId);
            return transaction;
        }

        public async Task<Transaction> TransferAmount(TransferDto transferDto)
        {
            // source, target, amount
            try
            {
                var sourceAccount = await _accountRepository.Get(transferDto.SourceAccountId);
                var targetAccount = await _accountRepository.Get(transferDto.TargetAccountId);
                if (sourceAccount != null && targetAccount != null)
                {
                    if (sourceAccount.Balance >= transferDto.Amount)
                    {
                        var withdraw = new WithdrawDto
                        {
                            AccountId = transferDto.SourceAccountId,
                            Amount = transferDto.Amount

                        };
                        await WithdrawAmount(withdraw);
                        var deposit = new DepositDto
                        {
                            AccountId = transferDto.TargetAccountId,
                            Amount = transferDto.Amount
                        };
                        await DepositAmount(deposit);
                        var transaction = new Transaction
                        {
                            AccountId = transferDto.SourceAccountId,
                            TargetAccountId = transferDto.TargetAccountId,
                            Amount = transferDto.Amount,
                            Type = TransactionType.Transfer,
                            Timestamp = DateTime.UtcNow
                        };
                        transaction = await _transactionRepository.Add(transaction);
                        return transaction;
                    }
                    else
                    {
                        throw new Exception("Insufficient funds.");
                    }
                }
                else
                {
                    throw new Exception("Account not found.");
                }
                
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            
        }

        public async Task<Transaction> WithdrawAmount(WithdrawDto withdrawDto)
        {
            var account = await _accountRepository.Get(withdrawDto.AccountId);
            if (account == null)
                throw new Exception("Account not found");
            account.Balance -= withdrawDto.Amount;
            var transaction = new Transaction
            {
                AccountId = withdrawDto.AccountId,
                Amount = withdrawDto.Amount,
                Type = TransactionType.Withdrawal,
                Timestamp = DateTime.UtcNow
            };
            transaction = await _transactionRepository.Add(transaction);
            return transaction;
        }
    }
}