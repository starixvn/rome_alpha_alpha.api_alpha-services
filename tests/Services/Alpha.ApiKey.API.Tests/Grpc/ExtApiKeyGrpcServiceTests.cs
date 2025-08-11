using System.Threading.Tasks;
using Alpha.ApiKey.API.Services;
using Grpc.Core;
using GrpcExtApiKey;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Alpha.ApiKey.API.Models;

namespace Alpha.ApiKey.API.Tests.Grpc
{
    public class ExtApiKeyGrpcServiceTests
    {
        [Fact]
        public async Task VerifyExtApiKey_ReturnsAuthenticatedResponse_WhenApiKeyIsValid()
        {
            // Arrange
            var apiKeyService = Substitute.For<IExtApiKeyService>();
            var logger = Substitute.For<ILogger<ExtApiKeyGrpcService>>();
            var grpcService = new ExtApiKeyGrpcService(apiKeyService, logger);
            var request = new VerifyExtApiKeyRequest { Key = "key", Username = "user", Password = "pass" };
            var context = TestServerCallContext.Create("VerifyExtApiKey", null, DateTime.UtcNow.AddMinutes(1), new Metadata(), CancellationToken.None, "127.0.0.1", null, null, null, new Dictionary<object, object>());
            apiKeyService.VerifyApiKey("key", "user", "pass").Returns(Task.FromResult(new VerifyExtApiKeyResult
            {
                IsAuthenticated = true,
                CustId = 123,
                SubAccId = 456,
                ErrMsg = null,
                Site = "site",
                Currency = 840 // USD numeric code for demonstration
            }));

            // Act
            var response = await grpcService.VerifyExtApiKey(request, context);

            // Assert
            Assert.True(response.IsAuthenticated);
            Assert.Equal(123, response.CustId);
            Assert.Equal(456, response.SubAccId);
            Assert.Equal("site", response.Site);
            Assert.Equal(840, response.Currency);
            Assert.Equal(string.Empty, response.ErrMsg);
        }

        [Fact]
        public async Task VerifyExtApiKey_ReturnsUnauthenticatedResponse_WhenApiKeyIsInvalid()
        {
            // Arrange
            var apiKeyService = Substitute.For<IExtApiKeyService>();
            var logger = Substitute.For<ILogger<ExtApiKeyGrpcService>>();
            var grpcService = new ExtApiKeyGrpcService(apiKeyService, logger);
            var request = new VerifyExtApiKeyRequest { Key = "badkey", Username = "user", Password = "pass" };
            var context = TestServerCallContext.Create("VerifyExtApiKey", null, DateTime.UtcNow.AddMinutes(1), new Metadata(), CancellationToken.None, "127.0.0.1", null, null, null, new Dictionary<object, object>());
            apiKeyService.VerifyApiKey("badkey", "user", "pass").Returns(Task.FromResult<VerifyExtApiKeyResult?>(null));

            // Act
            var response = await grpcService.VerifyExtApiKey(request, context);

            // Assert
            Assert.False(response.IsAuthenticated);
            Assert.Equal(0, response.CustId);
            Assert.Equal(0, response.SubAccId);
            Assert.Equal(string.Empty, response.Site);
            Assert.Equal(0, response.Currency);
        }
    }

    // Minimal TestServerCallContext for unit testing
    public class TestServerCallContext : ServerCallContext
    {
        private readonly Metadata _requestHeaders;
        private readonly CancellationToken _cancellationToken;
        private readonly string _peer;
        private readonly DateTime _deadline;
        private readonly string _method;

        private TestServerCallContext(string method, Metadata requestHeaders, DateTime deadline, Metadata responseTrailers, CancellationToken cancellationToken, string peer, AuthContext authContext, ContextPropagationToken contextPropagationToken, WriteOptions writeOptions, IDictionary<object, object> userState)
        {
            _method = method;
            _requestHeaders = requestHeaders ?? new Metadata();
            _deadline = deadline;
            _cancellationToken = cancellationToken;
            _peer = peer;
        }

        public static TestServerCallContext Create(string method, Metadata requestHeaders, DateTime deadline, Metadata responseTrailers, CancellationToken cancellationToken, string peer, AuthContext authContext, ContextPropagationToken contextPropagationToken, WriteOptions writeOptions, IDictionary<object, object> userState)
        {
            return new TestServerCallContext(method, requestHeaders, deadline, responseTrailers, cancellationToken, peer, authContext, contextPropagationToken, writeOptions, userState);
        }

        protected override string MethodCore => _method;
        protected override string HostCore => "localhost";
        protected override string PeerCore => _peer;
        protected override DateTime DeadlineCore => _deadline;
        protected override Metadata RequestHeadersCore => _requestHeaders;
        protected override CancellationToken CancellationTokenCore => _cancellationToken;
        protected override Metadata ResponseTrailersCore => new Metadata();
        protected override Status StatusCore { get; set; }
    protected override WriteOptions? WriteOptionsCore { get; set; }
    protected override AuthContext AuthContextCore => null!;
    protected override ContextPropagationToken CreatePropagationTokenCore(ContextPropagationOptions? options) => null!;
        protected override Task WriteResponseHeadersAsyncCore(Metadata responseHeaders) => Task.CompletedTask;
    }
}
