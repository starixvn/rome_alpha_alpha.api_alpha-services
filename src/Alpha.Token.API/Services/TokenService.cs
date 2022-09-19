using System.Text;
using System.Security.Cryptography;
using System.Text.Json;
using Alpha.Token.API.Models;
using Fanex.Caching;

namespace Alpha.Token.Alpha.Services;

public interface ITokenService
{
    Task<TokenResult> CreateToken(int custId, string apiName, string passPhrase, string[] scopes, DateTime? expirationDate);

    Task<VerifyTokenResult> VerifyToken(string apiKey, string passPhrase, string apiSignature, string timeSpan);

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

    public async Task<TokenResult> CreateToken(int custId, string apiName, string passPhrase, string[] scopes, DateTime? expirationDate)
    {
        var apiKeyBytes = RandomNumberGenerator.GetBytes(24);
        var apiSecretBytes = RandomNumberGenerator.GetBytes(32);
        var apiKey = Convert.ToBase64String(apiKeyBytes);
        var apiSecret = Convert.ToBase64String(apiSecretBytes);

        var apiSecrets = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.Tokens") ?? new List<string>();
        apiSecrets.Add(apiKey);

        using var hmac = new HMACSHA256(apiSecretBytes);
        var passPhraseEncrypt = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(passPhrase)));

        AccessToken accessToken = new(custId, apiName, apiKey, passPhraseEncrypt, apiSecret, scopes, expirationDate);
        await Task.WhenAll(
            cacheService.SetAsync($"{CacheKey}.{apiKey}", accessToken, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))),
            cacheService.SetAsync($"{CacheKey}.{custId}.Tokens", apiSecrets, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))));

        return new TokenResult(apiName, apiKey, apiSecret);
    }

    public async Task<VerifyTokenResult> VerifyToken(string apiKey, string passPhrase, string apiSignature, string timeSpan)
    {
        var accessToken = await Get(apiKey);
        if (accessToken == null)
        {
            return new VerifyTokenResult(false);
        }

        using var hmac = new HMACSHA256(Convert.FromBase64String(accessToken.ApiSecret));
        var signatureValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(timeSpan));
        var verifyApiKey = apiKey.Equals(accessToken.ApiKey);
        var verifyPassPhrase = passPhrase.Equals(accessToken.PassPhrase);
        var verifyApiSignature = apiSignature.Equals(Convert.ToBase64String(signatureValue));

        if (accessToken.IsExpired || !verifyApiKey || !verifyPassPhrase || !verifyApiSignature)
        {
            return new VerifyTokenResult(false);
        }

        return new VerifyTokenResult(true) { Scopes = accessToken.Scopes };
    }

    public async Task<IEnumerable<AccessToken>> GetUserTokens(int custId)
    {
        var accessTokens = new List<AccessToken>();
        var apiSecrets = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.Tokens");

        if (apiSecrets == null || !apiSecrets.Any())
        {
            return accessTokens;
        }

        foreach (var apiSecret in apiSecrets)
        {
            accessTokens.Add(await cacheService.GetAsync<AccessToken>($"{CacheKey}.{apiSecret}"));
        }

        return accessTokens;
    }

    public async Task<AccessToken> Get(string apiKey)
    {
        var accessToken = await cacheService.GetAsync<AccessToken>($"{CacheKey}.{apiKey}");
        return accessToken;
    }
}