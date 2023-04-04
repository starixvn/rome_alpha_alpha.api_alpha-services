using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Alpha.ApiGateways.Infrastructure;

public class GrpcExceptionInterceptor : Interceptor
{
    private readonly Fanex.Logging.ILogger _logger;

    public GrpcExceptionInterceptor(Fanex.Logging.ILogger logger)
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

            return default;
        }
    }
}