namespace RealEstateApi.Interfaces
{
    public interface ITokenBlacklistService
    {
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task AddToBlacklistAsync(string token, DateTime expiry);
    }

}