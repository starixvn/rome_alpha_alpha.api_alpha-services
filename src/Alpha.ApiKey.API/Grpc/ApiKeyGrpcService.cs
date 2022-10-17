using Alpha.ApiKey.API.Services;
using Grpc.Core;

namespace GrpcApiKey;

public class ApiKeyGrpcService : ApiKey.ApiKeyBase
{
    private readonly IApiKeyService apiKeyService;
    private readonly ILogger<ApiKeyGrpcService> logger;

    public ApiKeyGrpcService(IApiKeyService apiKeyService, ILogger<ApiKeyGrpcService> logger)
    {
        this.apiKeyService = apiKeyService;
        this.logger = logger;
    }

    public override async Task<VerifyApiKeyResponse> VerifyApiKey(VerifyApiKeyRequest request, ServerCallContext context)
    {
        logger.LogInformation("Begin grpc call from method {Method} for basket id {Id}", context.Method, request.Key);

        var result = await apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timespan);

        if (result != null)
        {
            var response = new VerifyApiKeyResponse { Isauthenticated = result.IsAuthenticated };

            foreach (var scope in result.Scopes)
            {
                response.Scopes.Add(scope);
            }

            return response;
        }

        context.Status = new Status(StatusCode.Unauthenticated, $"Verify api key {request.Key} failed");

        return new VerifyApiKeyResponse();
    }
}
