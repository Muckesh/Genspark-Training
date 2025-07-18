namespace RealEstateApi.Models
{
    public class Purchase
    {
        public Guid Id { get; set; }
        public Guid BuyerId { get; set; }
        public Guid ListingId { get; set; }

        public double PriceAtPurchase { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Completed"; // Pending / Cancelled

        public string DiscountCode { get; set; } = string.Empty; //modified model for discount feature

        public double OrginalPrice { get; set; }  //modified model for discount feature
        // Nav
        public User? Buyer { get; set; }
        public PropertyListing? Listing { get; set; }
    }
}