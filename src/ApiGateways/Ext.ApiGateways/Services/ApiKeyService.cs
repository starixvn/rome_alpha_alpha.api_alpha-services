using Ext.ApiGateways.Models;
using GrpcExtApiKey;

namespace Ext.ApiGateways.Services;

public interface IApiKeyService
{
    Task<VerifyApiKeyResult> VerifyApiKey(string key, string username, string password);
}

public class ApiKeyService : IApiKeyService
{
    private readonly ExtApiKey.ExtApiKeyClient apiKeyClient;
    private readonly ILogger<ApiKeyService> logger;

    public ApiKeyService(ExtApiKey.ExtApiKeyClient apiKeyClient, ILogger<ApiKeyService> logger)
    {
        this.apiKeyClient = apiKeyClient;
        this.logger = logger;
    }

    public async Task<VerifyApiKeyResult> VerifyApiKey(string key, string username, string password)
    {
        logger.LogDebug("grpc client created, api key = {@key}", key);
        var response = await apiKeyClient.VerifyExtApiKeyAsync(new VerifyExtApiKeyRequest { Key = key, Username = username, Password = password});
        logger.LogDebug("grpc response {@response}", response);

        return new VerifyApiKeyResult
        {
            IsAuthenticated = response.IsAuthenticated,
            CustId = response.CustId,
            SubAccId = response.SubAccId,
            SiteName = response.Site,
            Currency = response.Currency,
        };
    }
}