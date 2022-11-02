using System.Text;
using System.Security.Cryptography;
using Alpha.ApiKey.API.Models;
using Fanex.Data.Repository;
using Alpha.ApiKey.API.Criteria;

namespace Alpha.ApiKey.API.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IDynamicRepository dynamicRepository;

    public ApiKeyService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<ApiKeyResult> CreateApiKey(int custId, string apiName, string passPhrase, string scopes, DateTime? expirationDate)
    {
        var keyBytes = RandomNumberGenerator.GetBytes(12);
        var apiSecretBytes = RandomNumberGenerator.GetBytes(16);
        var key = Convert.ToHexString(keyBytes).ToLowerInvariant();
        var apiSecret = Convert.ToHexString(apiSecretBytes).ToLowerInvariant();
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret));
        var passPhraseEncrypt = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(passPhrase)));

        await dynamicRepository.ExecuteAsync(new CreateApiKeyCommand(custId, apiName, passPhraseEncrypt, scopes, key, apiSecret, expirationDate));

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

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKeyAccess.Secret));
        var signatureValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(timestamp));
        var verifyApiKey = key.Equals(apiKeyAccess.Key);
        var verifyPassPhrase = passPhrase.Equals(apiKeyAccess.PassPhrase);
        var verifyApiSignature = signature.Equals(Convert.ToBase64String(signatureValue));

        if (apiKeyAccess.IsExpired || !verifyApiKey || !verifyPassPhrase || !verifyApiSignature)
        {
            return new VerifyApiKeyResult(false);
        }

        return new VerifyApiKeyResult(true) { CustId = apiKeyAccess.CustId };
    }

    public Task<IEnumerable<ApiKeyEntity>> GetApiKeys(int custId)
        => dynamicRepository.FetchAsync<ApiKeyEntity>(new GetCustomerApiKeysCriteria(custId));

    public Task<ApiKeyEntity> Get(string key)
        => dynamicRepository.GetAsync<ApiKeyEntity>(new GetApiKeyCriteria(key));

    public Task RevokeApiKey(string key, int custId)
        => dynamicRepository.ExecuteAsync(new DeleteApiKeyCommand(key, custId));

    public Task UpdateApiKey(int custId, string key, string name, string scopes, DateTime? expirationDate)
        => dynamicRepository.ExecuteAsync(new UpdateApiKeyCommand(custId, key, name, scopes, expirationDate));
}