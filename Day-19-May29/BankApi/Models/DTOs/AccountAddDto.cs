namespace BankApi.Models.DTOs
{
    public class AccountAddDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}