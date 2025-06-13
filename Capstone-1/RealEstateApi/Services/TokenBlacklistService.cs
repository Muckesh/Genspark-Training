using System.Collections.Concurrent;
using RealEstateApi.Interfaces;

namespace RealEstateApi.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, DateTime> _blacklistedTokens = new();
        public Task AddToBlacklistAsync(string token, DateTime expiry)
        {
            _blacklistedTokens[token] = expiry;
            return Task.CompletedTask;
        }

        public Task<bool> IsTokenBlacklistedAsync(string token)
        {
            if (_blacklistedTokens.TryGetValue(token, out var expiry))
            {
                if (DateTime.UtcNow < expiry)
                    return Task.FromResult(true);
                _blacklistedTokens.TryRemove(token, out _);
            }
            return Task.FromResult(false);
        }
    }
}