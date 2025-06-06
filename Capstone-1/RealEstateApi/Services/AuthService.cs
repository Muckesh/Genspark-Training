using RealEstateApi.Interaces;
using RealEstateApi.Models.DTOs;

namespace RealEstateApi.Services
{
    public class AuthService : IAuthService
    {
        public Task<AuthResponseDto> LoginAsync(LoginDto login)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterAgentAsync(RegisterAgentDto registerAgent)
        {
            throw new NotImplementedException();
        }

        public Task<AuthResponseDto> RegisterBuyerAsync(RegisterBuyerDto registerBuyer)
        {
            throw new NotImplementedException();
        }
    }
}