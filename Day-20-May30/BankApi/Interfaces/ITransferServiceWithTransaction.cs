using BankApi.Models;
using BankApi.Models.DTOs;

namespace BankApi.Interfaces
{
    public interface ITransferServiceWithTransaction
    {
        Task<IEnumerable<Transaction>> Transfer(TransferDto transferDto);
    }
}