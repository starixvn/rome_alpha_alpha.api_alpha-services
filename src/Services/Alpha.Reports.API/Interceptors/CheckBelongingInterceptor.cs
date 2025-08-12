using System.Net;
using Alpha.Reports.API.Services;
using Alpha.Shared.Services.Contracts;
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
        // Check belonging status using shared helper
        var belongingResult = await CheckBelongingInterceptorHelper.CheckBelongingWithErrorHandling(
            request,
            belongingCheckService.CheckBelongingAsync,
            async (errorMessage) => await CheckBelongingInterceptorHelper.CreateForbiddenResponse(context, errorMessage));

        if (belongingResult)
        {
            return await base.UnaryServerHandler(request, context, continuation);
        }

        // Return forbidden status
        throw new RpcException(new Status(StatusCode.PermissionDenied, ((int)HttpStatusCode.Forbidden).ToString()));
    }
}