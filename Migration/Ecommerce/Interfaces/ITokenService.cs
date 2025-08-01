using Ecommerce.Models;

namespace Ecommerce.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(User user);
        public Task<string> GenerateRefreshToken();
    }
}