using System.Net;
using Alpha.BetLists.API.Requests;
using Grpc.Core;
using Grpc.Core.Interceptors;
using GrpcCustomer;
using System.Text.Json;

namespace Alpha.BetLists.API.Interceptors;

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
        if (request == null)
        {
            return true;
        }

        var baseRequest = JsonSerializer.Deserialize<BaseRequest>(request.ToString());
        if (baseRequest == null || baseRequest.ViewerId == 0 || string.IsNullOrWhiteSpace(baseRequest.CustName))
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
            await HandleForbiddenResponse(context, $"{baseRequest.CustName} is not belonging to ${baseRequest.ViewerId}");
        }

        return downlineInfo.IsBelonging;
    }
}