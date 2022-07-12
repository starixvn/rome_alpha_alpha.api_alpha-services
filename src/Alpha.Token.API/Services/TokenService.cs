using System.Security.Cryptography;
using System.Text.Json;
using Alpha.Token.API.Models;
using Fanex.Caching;

namespace Alpha.Token.Alpha.Services;

public interface ITokenService
{
    Task<string> CreateToken(int custId, string tokenName, string[] scopes, DateTime? expirationDate);

    Task<IEnumerable<AccessToken>> GetUserTokens(int custId);

    Task<AccessToken> Get(string token);
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
        var tokens = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.Tokens") ?? new List<string>();
        tokens.Add(token);
        AccessToken accessToken = new(custId, tokenName, token, scopes, expirationDate);
        await Task.WhenAll(
            cacheService.SetAsync($"{CacheKey}.{token}", accessToken, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))),
            cacheService.SetAsync($"{CacheKey}.{custId}.Tokens", tokens, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))));

        return token;
    }

    public async Task<IEnumerable<AccessToken>> GetUserTokens(int custId)
    {
        var accessTokens = new List<AccessToken>();
        var tokens = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.Tokens");

        if (tokens == null || !tokens.Any())
        {
            return accessTokens;
        }

        foreach (var token in tokens)
        {
            accessTokens.Add(await cacheService.GetAsync<AccessToken>($"{CacheKey}.{token}"));
        }

        return accessTokens;
    }

    public async Task<AccessToken> Get(string token)
    {
        var accessToken = await cacheService.GetAsync<AccessToken>($"{CacheKey}.{token}");
        Console.WriteLine(JsonSerializer.Serialize(accessToken));
        return accessToken;
    }
}