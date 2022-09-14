using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using Alpha.Token.API.Models;
using Fanex.Caching;

namespace Alpha.Token.Alpha.Services;

public interface ITokenService
{
    Task<TokenResult> CreateToken(int custId, string tokenName, string passPhrase, string[] scopes, DateTime? expirationDate);

    Task<VerifyTokenResult> VerifyToken(string apiKey, string passPhrase, string apiSign, string timeSpan);

    Task<IEnumerable<AccessToken>> GetUserTokens(int custId);

    Task<AccessToken> Get(string apiKey);
}

public class TokenService : ITokenService
{
    private const string CacheKey = "Alpha.Token.Alpha.Services.TokenService";
    private readonly ICacheService cacheService;

    public TokenService(ICacheService cacheService)
    {
        this.cacheService = cacheService;
    }

    public async Task<TokenResult> CreateToken(int custId, string tokenName, string passPhrase, string[] scopes, DateTime? expirationDate)
    {
        var apiKeyBytes = RandomNumberGenerator.GetBytes(24);
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var apiKey = Convert.ToBase64String(apiKeyBytes);
        var token = Convert.ToBase64String(tokenBytes);

        var tokens = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.Tokens") ?? new List<string>();
        tokens.Add(apiKey);

        using var hmac = new HMACSHA256(tokenBytes);
        var passPhraseEncrypt = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(passPhrase)));

        AccessToken accessToken = new(custId, tokenName, apiKey, passPhraseEncrypt, token, scopes, expirationDate);
        await Task.WhenAll(
            cacheService.SetAsync($"{CacheKey}.{apiKey}", accessToken, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))),
            cacheService.SetAsync($"{CacheKey}.{custId}.Tokens", tokens, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))));

        return new TokenResult(tokenName, apiKey, token);
    }

    public async Task<VerifyTokenResult> VerifyToken(string apiKey, string passPhrase, string apiSign, string timeSpan)
    {
        var accessToken = await Get(apiKey);
        if (accessToken == null)
        {
            return new VerifyTokenResult(false);
        }

        using var hmac = new HMACSHA256(Convert.FromBase64String(accessToken.Token));
        var signValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(timeSpan));
        var verifyApiKey = apiKey.Equals(accessToken.ApiKey);
        var verifyPassPhrase = passPhrase.Equals(accessToken.PassPhrase);
        var verifyApiSign = apiSign.Equals(Convert.ToBase64String(signValue));

        if (accessToken.IsExpired || !verifyApiKey || !verifyPassPhrase || !verifyApiSign)
        {
            return new VerifyTokenResult(false);
        }

        return new VerifyTokenResult(true) { Scopes = accessToken.Scopes };
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

    public async Task<AccessToken> Get(string apiKey)
    {
        var accessToken = await cacheService.GetAsync<AccessToken>($"{CacheKey}.{apiKey}");
        return accessToken;
    }
}