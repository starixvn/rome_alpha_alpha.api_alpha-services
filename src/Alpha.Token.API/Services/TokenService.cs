using System.Security.Cryptography;
using Alpha.Token.API.Models;
using Fanex.Caching;

namespace Alpha.Token.Alpha.Services;

public interface ITokenService
{
    Task<string> CreateToken(int custId, string tokenName, string[] scopes, DateTime? expirationDate);

    Task<IEnumerable<AccessToken>> GetUserTokens(int custId);
}

public class TokenService : ITokenService
{
    private const string CacheKey = "Alpha.Token.Alpha.Services.TokenService";
    private readonly ICacheService cacheService;

    public TokenService(ICacheService cacheService)
    {
        this.cacheService = cacheService;
    }

    public async Task<string> CreateToken(int custId, string tokenName, string[] scopes, DateTime? expirationDate)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var tokenNames = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.TokenNames") ?? new List<string>();
        tokenNames.Add(tokenName);
        AccessToken accessToken = new(custId, tokenName, token, scopes, expirationDate);
        await Task.WhenAll(
            cacheService.SetAsync($"{CacheKey}.{custId}.{tokenName}", accessToken),
            cacheService.SetAsync($"{CacheKey}.{custId}.TokenNames", tokenNames));

        return token;
    }

    public async Task<IEnumerable<AccessToken>> GetUserTokens(int custId)
    {
        var accessTokens = new List<AccessToken>();
        var tokenNames = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.TokenNames");

        if (tokenNames == null || !tokenNames.Any())
        {
            return accessTokens;
        }

        foreach (var tokenName in tokenNames)
        {
            accessTokens.Add(await cacheService.GetAsync<AccessToken>($"{CacheKey}.{custId}.{tokenName}"));
        }

        return accessTokens;
    }
}