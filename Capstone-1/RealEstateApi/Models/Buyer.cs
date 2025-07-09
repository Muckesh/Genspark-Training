namespace RealEstateApi.Models
{
    public class Buyer
    {
        public Guid Id { get; set; } // same as User.Id (1:1)
        public string PreferredLocation { get; set; } = string.Empty;
        public double Budget { get; set; }
        public string Phone { get; set; } = string.Empty;

        // Nav
        public User? User { get; set; }
        public ICollection<Inquiry>? Inquiries { get; set; }

    }
}