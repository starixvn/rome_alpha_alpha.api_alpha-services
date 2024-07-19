using Alpha.ApiKey.API.Services;
using Grpc.Core;

namespace GrpcExtApiKey;

public class ExtApiKeyGrpcService : ExtApiKey.ExtApiKeyBase
{
    private readonly IExtApiKeyService apiKeyService;
    private readonly ILogger<ExtApiKeyGrpcService> logger;

    public ExtApiKeyGrpcService(IExtApiKeyService apiKeyService, ILogger<ExtApiKeyGrpcService> logger)
    {
        this.apiKeyService = apiKeyService;
        this.logger = logger;
    }

    public override async Task<VerifyExtApiKeyResponse> VerifyExtApiKey(VerifyExtApiKeyRequest request, ServerCallContext context)
    {
        logger.LogDebug("Begin grpc call from method {Method} for verify api key {Id}", context.Method, request.Key);

        var result = await apiKeyService.VerifyApiKey(request.Key, request.Username, request.Password);

        if (result == null)
        {
            context.Status = new Status(StatusCode.Unauthenticated, $"Verify api key {request.Key} failed");

            return new VerifyExtApiKeyResponse();
        }

        context.Status = new Status(StatusCode.OK, $"Verify api key {request.Key} success");

        var response = new VerifyExtApiKeyResponse
        {
            IsAuthenticated = result.IsAuthenticated,
            CustId = result.CustId,
            SubAccId = result.SubAccId,
            ErrMsg = result.ErrMsg ?? string.Empty,
            Site = result.Site,
            Currency = result.Currency,
        };

        return response;
    }
}
