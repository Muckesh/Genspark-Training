using BankApi.Models.DTOs;

namespace BankApi.Interfaces
{
    public interface IFaqService
    {
        public Task<FAQResponseDto> AskQuestion(string question);
    }
}