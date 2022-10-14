using Alpha.ApiGateways.Extensions;
using Alpha.ApiGateways.Models;
using GrpcApiKeyClient;

namespace Alpha.ApiGateways.Services;

public interface IApiKeyService
{
    Task<VerifyApiKeyResult?> VerifyApiKey(string key, string passPhrase, string signature, string timeSpan);
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

    public async Task<VerifyApiKeyResult?> VerifyApiKey(string key, string passPhrase, string signature, string timeSpan)
    {
        logger.LogDebug("grpc client created, api key = {@key}", key);
        var response = await apiKeyClient.VerifyApiKeyAsync(new VerifyApiKeyRequest { Key = key, Passphrase = passPhrase, Signature = signature, Timespan = timeSpan });
        logger.LogDebug("grpc response {@response}", response);

        return new VerifyApiKeyResult { IsAuthenticated = response.Isauthenticated, Scopes = response.Scopes.ToArray() };
    }
}