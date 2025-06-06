namespace RealEstateApi.Models
{
    public class Agent
    {
        public Guid Id { get; set; } // same as User.Id (1:1)
        public string LicenseNumber { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // Nav
        public User? User { get; set; }
        public ICollection<PropertyListing>? Listings { get; set; }
    }
}