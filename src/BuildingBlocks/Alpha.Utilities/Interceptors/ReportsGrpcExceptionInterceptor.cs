using System.Net;
using Alpha.Utilities.Models;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Alpha.Utilities.Infrastructure;

public class ReportsGrpcExceptionInterceptor : Interceptor
{
    private readonly Fanex.Logging.ILogger _logger;

    public ReportsGrpcExceptionInterceptor(Fanex.Logging.ILogger logger)
    {
        _logger = logger;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(HandleResponse(call.ResponseAsync), call.ResponseHeadersAsync, call.GetStatus, call.GetTrailers, call.Dispose);
    }

    private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> task)
    {
        try
        {
            var response = await task;

            return response;
        }
        catch (RpcException e)
        {
            await _logger.ErrorAsync($"Error calling via grpc: {e.Status} - {e.Message}", e);

            throw new GrpcException
            {
                Code = (int)MapStatusCode(e.Status.StatusCode),
                Detail = string.Empty
            };
        }
    }

    private static HttpStatusCode MapStatusCode(StatusCode code)
        => code switch
        {
            StatusCode.Unavailable => HttpStatusCode.ServiceUnavailable,
            StatusCode.PermissionDenied => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError,
        };
}