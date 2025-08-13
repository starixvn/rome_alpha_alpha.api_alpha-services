using Alpha.BetLists.API.Services;
using Alpha.Shared.Services.Interceptors;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Alpha.BetLists.API.Interceptors;

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
        await CheckBelongingInterceptorHelper.CheckBelongingWithErrorHandling(
            request,
            belongingCheckService.CheckBelongingAsync,
            async (errorMessage) => await CheckBelongingInterceptorHelper.CreateForbiddenResponse(context, errorMessage));

        return await base.UnaryServerHandler(request, context, continuation);
    }
}