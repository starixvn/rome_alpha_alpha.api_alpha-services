using Alpha.ApiKey.API.Models;

namespace Alpha.ApiKey.API.Services;

public interface IApiKeyService
{
    Task<CreateApiKeyResult> CreateApiKey(int custId, string apiName, string passPhrase, string scopes, string ip, string language, DateTime? expirationDate);

    Task<VerifyApiKeyResult> VerifyApiKey(string key, string passPhrase, string signature, string timestamp);

    Task<IEnumerable<ApiKeyEntity>> GetApiKeys(int custId);

    Task<Result> UpdateApiKey(int custId, string key, string name, string scopes, string ip, string language, DateTime? expirationDate);

    Task RevokeApiKey(string key, int custId, string ip);

    Task<ApiKeyEntity> Get(string key);
}
