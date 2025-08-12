using System.Net;
using Alpha.Reports.API.Requests;
using Alpha.Shared.Services.Interceptors;
using Grpc.Core;
using Grpc.Core.Interceptors;
using GrpcCustomer;
using Newtonsoft.Json;

namespace Alpha.Reports.API.Interceptors;

public class CheckBelongingInterceptor : Interceptor
{
    private readonly Customer.CustomerClient customerClient;

    public CheckBelongingInterceptor(Customer.CustomerClient customerClient)
    {
        this.customerClient = customerClient;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var isBelonging = await CheckBelonging(request, context);
        if (isBelonging)
        {
            var response = await base.UnaryServerHandler(request, context, continuation);
            return response;
        }

        throw new RpcException(new Status(StatusCode.PermissionDenied, ((int)HttpStatusCode.Forbidden).ToString()));
    }

    private static Task HandleForbiddenResponse(ServerCallContext context, string message)
    {
        var httpContext = context.GetHttpContext();
        httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return httpContext.Response.WriteAsync($"Forbidden: {message}");
    }

    private async Task<bool> CheckBelonging<TRequest>(TRequest request, ServerCallContext context)
    {
        if (BelongingCheckHelper.ShouldSkipBelongingCheck(request))
        {
            return true;
        }

        var baseRequest = JsonConvert.DeserializeObject<BaseRequest>(request.ToString());
        if (!BelongingCheckHelper.IsBaseRequestValid(baseRequest))
        {
            return true;
        }

        var getDownlineRequest = new GetDownlineInfoRequest
        {
            ViewerId = baseRequest.ViewerId,
            DownlineUsername = baseRequest.CustName
        };

        var downlineInfo = await customerClient.GetDownlineInfoAsync(getDownlineRequest);

        if (!downlineInfo.IsBelonging)
        {
            var errorMessage = BelongingCheckHelper.FormatBelongingErrorMessage(baseRequest);
            await HandleForbiddenResponse(context, errorMessage);
        }

        return downlineInfo.IsBelonging;
    }
}