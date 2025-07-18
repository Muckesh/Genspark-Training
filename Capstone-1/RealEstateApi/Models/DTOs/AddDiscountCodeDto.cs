namespace RealEstateApi.Models
{
    public class AddDiscountCodeDto
    {
        public string Code { get; set; } = string.Empty;

        public DateTime ExpiryDate { get; set; }

        public int Remaining { get; set; }

        public int DiscountPercentage { get; set; } // 1% to 100%


        // public string Status { get; set; } = string.Empty; //expired , available 

        

    }
}