using System.Threading.Tasks;
using Alpha.ApiKey.API.Models;
using Alpha.ApiKey.API.Services;
using Alpha.ApiKey.API.Tests.Fixtures;
using Grpc.Core;
using GrpcApiKey;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Alpha.ApiKey.API.Tests.Grpc;

public class ApiKeyGrpcServiceTests
{
    private readonly IApiKeyService apiKeyService = Substitute.For<IApiKeyService>();
    private readonly ILogger<ApiKeyGrpcService> logger = Substitute.For<ILogger<ApiKeyGrpcService>>();
    private readonly ApiKeyGrpcService service;
    private readonly ServerCallContext context = Substitute.For<ServerCallContext>();

    public ApiKeyGrpcServiceTests()
    {
        service = new ApiKeyGrpcService(apiKeyService, logger);
    }

    [Fact]
    public async Task VerifyApiKey_WithValidRequest_ReturnsSuccessResponse()
    {
        // Arrange
        var request = new VerifyApiKeyRequest
        {
            Key = "testkey123",
            Passphrase = "testpass",
            Signature = "validsignature",
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
        };

        var serviceResult = TestDataFixtures.VerifyApiKeyResults.ValidResult;
        apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timestamp)
            .Returns(serviceResult);

        // Act
        var response = await service.VerifyApiKey(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsAuthenticated);
        Assert.Equal(serviceResult.CustId, response.CustId);
        Assert.Equal(serviceResult.RoleId, response.RoleId);
        Assert.Equal(serviceResult.ErrorMessage, response.ErrorMessage);
        Assert.Contains("1", response.Scopes);
        Assert.Contains("2", response.Scopes);
        Assert.Contains("3", response.Scopes);
    }

    [Fact]
    public async Task VerifyApiKey_WithInvalidRequest_ReturnsFailureResponse()
    {
        // Arrange
        var request = new VerifyApiKeyRequest
        {
            Key = "invalidkey",
            Passphrase = "wrongpass",
            Signature = "invalidsignature",
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
        };

        var serviceResult = TestDataFixtures.VerifyApiKeyResults.InvalidResult;
        apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timestamp)
            .Returns(serviceResult);

        // Act
        var response = await service.VerifyApiKey(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsAuthenticated);
        Assert.Equal("Invalid signature", response.ErrorMessage);
    }

    [Fact]
    public async Task VerifyApiKey_WithNullResult_ReturnsEmptyResponse()
    {
        // Arrange
        var request = new VerifyApiKeyRequest
        {
            Key = "testkey123",
            Passphrase = "testpass",
            Signature = "signature",
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
        };

        apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timestamp)
            .Returns((VerifyApiKeyResult)null);

        // Act
        var response = await service.VerifyApiKey(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsAuthenticated);
        Assert.Equal(0, response.CustId);
        Assert.Equal(0, response.RoleId);
    }

    [Fact]
    public async Task VerifyApiKey_WithExpiredSignature_ReturnsExpiredResponse()
    {
        // Arrange
        var request = new VerifyApiKeyRequest
        {
            Key = "testkey123",
            Passphrase = "testpass",
            Signature = "signature",
            Timestamp = DateTimeOffset.Now.AddMinutes(-10).ToUnixTimeMilliseconds().ToString()
        };

        var serviceResult = TestDataFixtures.VerifyApiKeyResults.ExpiredResult;
        apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timestamp)
            .Returns(serviceResult);

        // Act
        var response = await service.VerifyApiKey(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.IsAuthenticated);
        Assert.Equal("Time expired!", response.ErrorMessage);
    }

    [Fact]
    public async Task VerifyApiKey_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var request = new VerifyApiKeyRequest
        {
            Key = "testkey123",
            Passphrase = "testpass",
            Signature = "signature",
            Timestamp = "1234567890"
        };

        // Act
        await service.VerifyApiKey(request, context);

        // Assert
        await apiKeyService.Received(1).VerifyApiKey(
            request.Key,
            request.Passphrase,
            request.Signature,
            request.Timestamp);
    }

    [Fact]
    public async Task GetApiKey_WithValidRequest_ReturnsApiKeyResponse()
    {
        // Arrange
        var request = new GetApiKeyRequest
        {
            Key = "testkey123"
        };

        var serviceResult = TestDataFixtures.ApiKeyEntities.ValidApiKey;
        apiKeyService.Get(request.Key).Returns(serviceResult);

        // Act
        var response = await service.GetApiKey(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(serviceResult.CustId, response.CustId);
        Assert.Equal(serviceResult.RoleId, response.RoleId);
    }

    [Fact]
    public async Task GetApiKey_WithNonExistentKey_ThrowsNullReferenceException()
    {
        // Arrange
        var request = new GetApiKeyRequest
        {
            Key = "nonexistentkey"
        };

        apiKeyService.Get(request.Key).Returns((ApiKeyEntity)null);

        // Act & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() => 
            service.GetApiKey(request, context));
    }

    [Fact]
    public async Task GetApiKey_CallsServiceWithCorrectParameter()
    {
        // Arrange
        var request = new GetApiKeyRequest
        {
            Key = "testkey123"
        };

        var apiKeyEntity = new ApiKeyEntity
        {
            CustId = 123,
            RoleId = 45
        };

        apiKeyService.Get(request.Key).Returns(apiKeyEntity);

        // Act
        await service.GetApiKey(request, context);

        // Assert
        await apiKeyService.Received(1).Get(request.Key);
    }

    [Fact]
    public async Task VerifyApiKey_WithEmptyScopes_HandlesCorrectly()
    {
        // Arrange
        var request = new VerifyApiKeyRequest
        {
            Key = "testkey123",
            Passphrase = "testpass",
            Signature = "signature",
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
        };

        var serviceResult = new VerifyApiKeyResult(true)
        {
            CustId = 12345,
            RoleId = 3,
            Scopes = "" // Empty scopes
        };

        apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timestamp)
            .Returns(serviceResult);

        // Act
        var response = await service.VerifyApiKey(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsAuthenticated);
        Assert.Single(response.Scopes);
        Assert.Equal("", response.Scopes[0]);
    }

    [Fact]
    public async Task VerifyApiKey_WithNullScopes_HandlesCorrectly()
    {
        // Arrange
        var request = new VerifyApiKeyRequest
        {
            Key = "testkey123",
            Passphrase = "testpass",
            Signature = "signature",
            Timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()
        };

        var serviceResult = new VerifyApiKeyResult(true)
        {
            CustId = 12345,
            RoleId = 3,
            Scopes = null // Null scopes
        };

        apiKeyService.VerifyApiKey(request.Key, request.Passphrase, request.Signature, request.Timestamp)
            .Returns(serviceResult);

        // Act
        var response = await service.VerifyApiKey(request, context);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsAuthenticated);
        Assert.Empty(response.Scopes);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(service);
    }
}
