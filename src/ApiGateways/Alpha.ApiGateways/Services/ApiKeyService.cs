using Alpha.ApiGateways.Models;
using GrpcApiKey;

namespace Alpha.ApiGateways.Services;

public interface IApiKeyService
{
    Task<VerifyApiKeyResult> VerifyApiKey(string key, string passPhrase, string signature, string timestamp);
    Task<GetApiKeyResult> GetApiKey(string key);
}

public class ApiKeyService : IApiKeyService
{
    private readonly ApiKey.ApiKeyClient apiKeyClient;
    private readonly ILogger<ApiKeyService> logger;

    public ApiKeyService(ApiKey.ApiKeyClient apiKeyClient, ILogger<ApiKeyService> logger)
    {
        this.apiKeyClient = apiKeyClient;
        this.logger = logger;
    }

    public async Task<VerifyApiKeyResult> VerifyApiKey(string key, string passPhrase, string signature, string timestamp)
    {
        logger.LogDebug("grpc client created, api key = {@key}", key);
        var response = await apiKeyClient.VerifyApiKeyAsync(new VerifyApiKeyRequest { Key = key, Passphrase = passPhrase, Signature = signature, Timestamp = timestamp });
        logger.LogDebug("grpc response {@response}", response);

        return new VerifyApiKeyResult
        {
            IsAuthenticated = response.IsAuthenticated,
            CustId = response.CustId,
            RoleId = response.RoleId,
            ErrorMessage = response.ErrorMessage,
            Scopes = response.Scopes.ToArray()
        };
    }

    public async Task<GetApiKeyResult> GetApiKey(string key)
    {
        var response = await apiKeyClient.GetApiKeyAsync(new GetApiKeyRequest { Key = key });

        return new GetApiKeyResult
        {
            CustId = response.CustId,
            RoleId = (byte)response.RoleId,
        };
    }
}