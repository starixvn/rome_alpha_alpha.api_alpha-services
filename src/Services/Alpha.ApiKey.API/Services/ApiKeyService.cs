using System.Text;
using System.Security.Cryptography;
using Alpha.ApiKey.API.Models;
using Fanex.Data.Repository;
using Alpha.ApiKey.API.Criteria;
using Microsoft.Extensions.Options;

namespace Alpha.ApiKey.API.Services;

public class ApiKeyService : IApiKeyService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly IOptions<APIKeyAppSettings> settings;

    public ApiKeyService(IDynamicRepository dynamicRepository, IOptions<APIKeyAppSettings> settings)
    {
        this.dynamicRepository = dynamicRepository;
        this.settings = settings;
    }

    public async Task<CreateApiKeyResult> CreateApiKey(int custId, string apiName, string passPhrase, string scopes, string ip, string language, DateTime? expirationDate)
    {
        var keyBytes = RandomNumberGenerator.GetBytes(12);
        var apiSecretBytes = RandomNumberGenerator.GetBytes(16);
        var key = Convert.ToHexString(keyBytes).ToLowerInvariant();
        var apiSecret = Convert.ToHexString(apiSecretBytes).ToLowerInvariant();
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret));
        var passPhraseEncrypt = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(passPhrase)));

        var criteria = new CreateApiKeyCommand(custId, apiName, passPhraseEncrypt, scopes, key, apiSecret, ip, language, expirationDate);
        await dynamicRepository.ExecuteAsync(criteria);

        if (criteria.ErrCode > 0)
        {
            return new CreateApiKeyResult { Result = new Result(criteria.ErrCode, criteria.ErrMsg) };
        }

        return new CreateApiKeyResult { ApiKeyResult = new ApiKeyResult(apiName, key, apiSecret) };
    }

    public async Task<VerifyApiKeyResult> VerifyApiKey(string key, string passPhrase, string signature, string timestamp)
    {
        var now = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (!long.TryParse(timestamp, out long requestTimeSpan) ||
            (now - requestTimeSpan) > settings.Value.ExpireSignatureTime ||
            (requestTimeSpan - now) > settings.Value.AllowSignatureTimeTimeRange)
        {
            return new VerifyApiKeyResult(false, "Time expired!");
        }

        var apiKey = await Get(key);
        if (apiKey == null)
        {
            return new VerifyApiKeyResult(false, "Key not found!");
        }

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiKey.Secret));
        var signatureValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(timestamp));
        var passPhraseInvalid = !passPhrase.Equals(apiKey.PassPhrase);
        var signatureInvalid = !signature.Equals(Convert.ToBase64String(signatureValue));
        
        if (apiKey.IsExpired)
        {
            return new VerifyApiKeyResult(false, "Key expired");
        }
  
        if (passPhraseInvalid)
        {
            return new VerifyApiKeyResult(false, "Pass phrase is invalid: " + passPhrase + " vs " + apiKey.PassPhrase);
        }

        if (signatureInvalid)
        {
            return new VerifyApiKeyResult(false, "Signature is invalid: " + signature + " vs " + Convert.ToBase64String(signatureValue));
        }
        
        return new VerifyApiKeyResult(true) { CustId = apiKey.CustId, RoleId = apiKey.RoleId, Scopes = apiKey.ScopeIdList };
    }

    public async Task<IEnumerable<ApiKeyEntity>> GetApiKeys(int custId)
        => await dynamicRepository.FetchAsync<ApiKeyEntity>(new GetCustomerApiKeysCriteria(custId));

    public async Task<ApiKeyEntity> Get(string key)
        => await dynamicRepository.GetAsync<ApiKeyEntity>(new GetApiKeyCriteria(key));

    public async Task RevokeApiKey(string key, int custId, string ip)
        => await dynamicRepository.ExecuteAsync(new DeleteApiKeyCommand(key, custId, ip));

    public async Task<Result> UpdateApiKey(int custId, string key, string name, string scopes, string ip, string language, DateTime? expirationDate)
    {
        var criteria = new UpdateApiKeyCommand(custId, key, name, scopes, ip, language, expirationDate);
        await dynamicRepository.ExecuteAsync(criteria);

        return new Result(criteria.ErrCode, criteria.ErrMsg);
    }
}