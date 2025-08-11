using System.Threading.Tasks;
using Alpha.Utilities.Interceptors;
using Fanex.Logging;
using Grpc.Core;
using Grpc.Core.Interceptors;
using NSubstitute;
using Xunit;

namespace Alpha.Utilities.Tests.Interceptors;

public class GrpcExceptionInterceptorTests
{
    [Fact]
    public void Constructor_SetsLogger()
    {
        var logger = Substitute.For<ILogger>();
        var interceptor = new GrpcExceptionInterceptor(logger);
        Assert.NotNull(interceptor);
    }

    [Fact]
    public async Task AsyncUnaryCall_WhenNoException_ReturnsResponse()
    {
        var logger = Substitute.For<ILogger>();
        var interceptor = new GrpcExceptionInterceptor(logger);
        var expected = "response";
        var call = new AsyncUnaryCall<string>(Task.FromResult(expected), null, null, null, () => { });
        var continuation = Substitute.For<AsyncUnaryCallContinuation<string, string>>();
        continuation.Invoke(Arg.Any<string>(), Arg.Any<ClientInterceptorContext<string, string>>()).Returns(call);
        var context = Substitute.For<ClientInterceptorContext<string, string>>(null, null, null);
        var resultCall = interceptor.AsyncUnaryCall("request", context, continuation);
        var result = await resultCall.ResponseAsync;
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task AsyncUnaryCall_WhenRpcException_LogsAndReturnsDefault()
    {
        var logger = Substitute.For<ILogger>();
        var interceptor = new GrpcExceptionInterceptor(logger);
        var rpcEx = new RpcException(new Status(StatusCode.Internal, "error"));
        var call = new AsyncUnaryCall<string>(Task.FromException<string>(rpcEx), null, null, null, () => { });
        var continuation = Substitute.For<AsyncUnaryCallContinuation<string, string>>();
        continuation.Invoke(Arg.Any<string>(), Arg.Any<ClientInterceptorContext<string, string>>()).Returns(call);
        var context = Substitute.For<ClientInterceptorContext<string, string>>(null, null, null);
        var resultCall = interceptor.AsyncUnaryCall("request", context, continuation);
        var result = await resultCall.ResponseAsync;
        Assert.Null(result);
        await logger.Received().ErrorAsync(Arg.Any<string>(), rpcEx);
    }
}
