using RealEstateApi.Interfaces;

namespace RealEstateApi.Services
{
    public class PasswordService : IPasswordService
    {
        public string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword,hashedPassword);
        }
    }
}