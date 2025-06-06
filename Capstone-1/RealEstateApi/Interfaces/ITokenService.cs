using RealEstateApi.Models;

namespace RealEstateApi.Interaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
    }
}