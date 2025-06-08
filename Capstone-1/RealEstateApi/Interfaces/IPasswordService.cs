
namespace RealEstateApi.Interfaces
{
    public interface IPasswordService
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string plainPassword, string hashedPassword);
    }
}