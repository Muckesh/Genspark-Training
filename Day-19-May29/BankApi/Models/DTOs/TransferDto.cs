namespace BankApi.Models.DTOs
{
    public class TransferDto
    {
        public Guid SourceAccountId { get; set; }
        public Guid TargetAccountId { get; set; }
        public decimal Amount { get; set; }
    }   
}