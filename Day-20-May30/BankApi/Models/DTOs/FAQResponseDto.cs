namespace BankApi.Models.DTOs
{
    public class FAQResponseDto
    {
        public string Answer { get; set; } = string.Empty;
        public float Confidence { get; set; }
    }
}