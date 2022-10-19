using System.Text;
using System.Security.Cryptography;
using Alpha.ApiKey.API.Models;
using Fanex.Caching;

namespace Alpha.ApiKey.API.Services;

public class ApiKeyService : IApiKeyService
{
    private const string CacheKey = "Alpha.ApiKey.API.Services.TokenService";
    private readonly ICacheService cacheService;

    public ApiKeyService(ICacheService cacheService)
    {
        this.cacheService = cacheService;
    }

    public async Task<ApiKeyResult> CreateApiKey(int custId, byte roleId, string apiName, string passPhrase, string[] scopes, DateTime? expirationDate, string restrictIps)
    {
        var keyBytes = RandomNumberGenerator.GetBytes(24);
        var apiSecretBytes = RandomNumberGenerator.GetBytes(32);
        var key = Convert.ToBase64String(keyBytes);
        var apiSecret = Convert.ToBase64String(apiSecretBytes);

        var apiSecrets = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.ApiKeys") ?? new List<string>();
        apiSecrets.Add(key);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret));
        var passPhraseEncrypt = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(passPhrase)));

        ApiKeyAccess accessToken = new(custId, roleId, apiName, key, passPhraseEncrypt, apiSecret, scopes, expirationDate);
        await Task.WhenAll(
            cacheService.SetAsync($"{CacheKey}.{key}", accessToken, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))),
            cacheService.SetAsync($"{CacheKey}.{custId}.ApiKeys", apiSecrets, new CacheItemOptions().SetAbsoluteExpiration(new TimeSpan(1, 0, 0))));

        return new ApiKeyResult(apiName, key, apiSecret);
    }

    public async Task<VerifyApiKeyResult> VerifyApiKey(string key, string passPhrase, string signature, string timestamp)
    {
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (!long.TryParse(timestamp, out long requestTimeSpan) || (now - requestTimeSpan) > 5000)
        {
            return new VerifyApiKeyResult(false);
        }

        var apiKeyAccess = await Get(key);
        if (apiKeyAccess == null)
        {
            return new VerifyApiKeyResult(false);
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKeyAccess.ApiSecret));
        var signatureValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(timestamp));
        var verifyApiKey = key.Equals(apiKeyAccess.Key);
        var verifyPassPhrase = passPhrase.Equals(apiKeyAccess.PassPhrase);
        var verifyApiSignature = signature.Equals(Convert.ToBase64String(signatureValue));

        if (apiKeyAccess.IsExpired || !verifyApiKey || !verifyPassPhrase || !verifyApiSignature)
        {
            return new VerifyApiKeyResult(false);
        }

        return new VerifyApiKeyResult(true) { CustId = apiKeyAccess.CustId, RoleId = apiKeyAccess.RoleId, Scopes = apiKeyAccess.Scopes };
    }

    public async Task<IEnumerable<ApiKeyAccess>> GetUserApiKeyAccess(int custId)
    {
        var apiKeyAccess = new List<ApiKeyAccess>();
        var apiSecrets = await cacheService.GetAsync<List<string>>($"{CacheKey}.{custId}.ApiKeys");

        if (apiSecrets == null || !apiSecrets.Any())
        {
            return apiKeyAccess;
        }

        foreach (var apiSecret in apiSecrets)
        {
            apiKeyAccess.Add(await cacheService.GetAsync<ApiKeyAccess>($"{CacheKey}.{apiSecret}"));
        }

        return apiKeyAccess;
    }

    public async Task<ApiKeyAccess> Get(string key)
    {
        var apiKeyAccess = await cacheService.GetAsync<ApiKeyAccess>($"{CacheKey}.{key}");
        return apiKeyAccess;
    }

    public Task RevokeApiKey(int custId, string key)
    {
        throw new NotImplementedException();
    }

    public Task<ApiKeyAccess> UpdateApiKey(int custId, ApiKeyAccess apiKeyAccess)
    {
        throw new NotImplementedException();
    }

    public Task<ApiKeyAccess> RenewApiKey(int custId, string key, DateTime expirationDate)
    {
        throw new NotImplementedException();
    }
}