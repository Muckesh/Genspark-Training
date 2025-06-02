namespace BankApi.Models.DTOs
{
    public class DepositDto
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
    }
}