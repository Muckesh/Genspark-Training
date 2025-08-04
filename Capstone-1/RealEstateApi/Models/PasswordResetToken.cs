namespace RealEstateApi.Models
{
    public class PasswordResetToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiry { get; set; }

        public User? User { get; set; }
    }

}