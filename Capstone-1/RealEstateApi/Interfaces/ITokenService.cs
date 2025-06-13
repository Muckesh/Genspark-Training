using RealEstateApi.Models;

namespace RealEstateApi.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
        public Task<string> GenerateRefreshToken();
    }
}