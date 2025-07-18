namespace RealEstateApi.Models.DTOs
{
    public class CreatePurchaseWithDiscountDto
    {
        public Guid ListingId { get; set; }

        public string code { get; set; } = string.Empty;
    }
}