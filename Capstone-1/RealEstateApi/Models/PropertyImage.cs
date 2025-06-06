namespace RealEstateApi.Models
{
    public class PropertyImage
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;

        public Guid PropertyListingId { get; set; }
        public PropertyListing? Listing { get; set; }
    }
}