using Alpha.Reports.API.Services;
using Alpha.Shared.Services.Interceptors;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Alpha.Reports.API.Interceptors;

public class CheckBelongingInterceptor : Interceptor
{
    private readonly IBelongingCheckService belongingCheckService;

    public CheckBelongingInterceptor(IBelongingCheckService belongingCheckService)
    {
        this.belongingCheckService = belongingCheckService;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // Check belonging status using shared helper - throws RpcException if forbidden
        await CheckBelongingInterceptorHelper.CheckBelongingWithErrorHandling(
            request,
            belongingCheckService.CheckBelongingAsync,
            async (errorMessage) => await CheckBelongingInterceptorHelper.CreateForbiddenResponse(context, errorMessage));

        // If we reach here, belonging check passed
        return await base.UnaryServerHandler(request, context, continuation);
    }
}