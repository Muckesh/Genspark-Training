using BankApi.Interfaces;
using BankApi.Models.DTOs;

namespace BankApi.Services
{
    public class FaqService : IFaqService
    {
        private readonly HttpClient _httpClient;

        public FaqService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<FAQResponseDto> AskQuestion(string question)
        {
            var request = new FAQRequestDto { Question = question };
            var response = await _httpClient.PostAsJsonAsync("http://localhost:8000/ask", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<FAQResponseDto>() ?? throw new Exception("Error occurred.");
        }
    }
}