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
        logger.LogInformation("Begin grpc call from method {Method} for verify api key {Id}", context.Method, request.Key);

        var result = await apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timestamp);

        if (result != null)
        {
            context.Status = new Status(StatusCode.OK, $"Verify api key {request.Key} success");

            var response = new VerifyApiKeyResponse
            {
                IsAuthenticated = result.IsAuthenticated,
                CustId = result.CustId,
                RoleId = result.RoleId
            };
            result.Scopes?.Split(",")?.ToList().ForEach(scope => response.Scopes.Add(scope));

            return response;
        }

        context.Status = new Status(StatusCode.Unauthenticated, $"Verify api key {request.Key} failed");

        return new VerifyApiKeyResponse();
    }

    public override async Task<GetApiKeyResponse> GetApiKey(GetApiKeyRequest request, ServerCallContext context)
    {
        var result = await apiKeyService.Get(request.Key);

        return new GetApiKeyResponse
        {
            CustId = result.CustId,
            RoleId = result.RoleId
        };
    }
}
