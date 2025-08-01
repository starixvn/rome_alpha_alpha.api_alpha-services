using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Alpha.Utilities.Interceptors;

public class GrpcServerExceptionInterceptor : Interceptor
{
    private readonly Fanex.Logging.ILogger _logger;

    public GrpcServerExceptionInterceptor(Fanex.Logging.ILogger logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (Exception ex)
        {
            await _logger.ErrorAsync($"Error thrown by {context.Method}.", ex);

            throw;
        }
    }
}