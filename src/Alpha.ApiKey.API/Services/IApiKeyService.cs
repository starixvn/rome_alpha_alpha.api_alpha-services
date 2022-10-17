using Alpha.ApiKey.API.Models;

namespace Alpha.ApiKey.API.Services;

public interface IApiKeyService
{
    Task<ApiKeyResult> CreateApiKey(int custId, byte roleId, string apiName, string passPhrase, string[] scopes, DateTime? expirationDate, string restrictIps);

    Task<VerifyApiKeyResult> VerifyApiKey(string key, string passPhrase, string signature, string timestamp);

    Task<IEnumerable<ApiKeyAccess>> GetUserApiKeyAccess(int custId);

    Task<ApiKeyAccess> UpdateApiKey(int custId, ApiKeyAccess apiKeyAccess);

    Task RevokeApiKey(int custId, string key);

    Task<ApiKeyAccess> RenewApiKey(int custId, string key, DateTime expirationDate);

    Task<ApiKeyAccess> Get(string key);
}
