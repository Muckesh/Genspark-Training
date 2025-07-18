namespace RealEstateApi.Models
{
    public class DiscountCodes
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public int Remaining { get; set; }

        public int DiscountPercentage { get; set; } // 1% to 100%

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsDeleted { get; set; } = false;

        
        public DateTime DeletedAt { get; set; } 

        public string Status { get; set; } = string.Empty; //expired , available 

        

    }
}