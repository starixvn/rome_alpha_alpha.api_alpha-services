using Alpha.ApiKey.API.Models;
using Alpha.ApiKey.API.Services;
using Alpha.Utilities.Helpers;
using Fanex.Caching;
using Fanex.Data.Repository;
using NSubstitute;

namespace Alpha.ApiKey.API.Tests.Services;

public class ExtApiKeyServiceTests
{
    private readonly IDynamicRepository mockDynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly ICacheService mockCacheService = Substitute.For<ICacheService>();
    private readonly ExtApiKeyService service;

    public ExtApiKeyServiceTests()
    {
        service = new ExtApiKeyService(mockDynamicRepository, mockCacheService);
    }

    [Fact]
    public async Task VerifyApiKey_WithCachedAuthenticatedResult_ReturnsCachedResult()
    {
        // Arrange
        var key = "testkey";
        var username = "testuser";
        var password = "testpass";
        var cachedKey = $"{key}_{username}";
        
        var cachedResult = new VerifyExtApiKeyResult
        {
            IsAuthenticated = true,
            CustId = 123,
            SubAccId = 456,
            Site = "testsite",
            Currency = 1
        };

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(cachedKey).Returns(cachedResult);

        // Act
        var result = await service.VerifyApiKey(key, username, password);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cachedResult, result);
        Assert.True(result.IsAuthenticated);
        Assert.Equal(123, result.CustId);
        Assert.Equal(456, result.SubAccId);
        Assert.Equal("testsite", result.Site);
        Assert.Equal(1, result.Currency);
    }

    [Fact]
    public async Task VerifyApiKey_WithCachedUnauthenticatedResult_ProcessesNewVerification()
    {
        // Arrange
        var key = "testkey";
        var username = "testuser";
        var password = "testpass";
        var cachedKey = $"{key}_{username}";
        
        var cachedResult = new VerifyExtApiKeyResult
        {
            IsAuthenticated = false
        };

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(cachedKey).Returns(cachedResult);
        mockDynamicRepository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(1));

        // Act
        var result = await service.VerifyApiKey(key, username, password);

        // Assert
        Assert.NotNull(result);
        await mockDynamicRepository.Received(1).ExecuteAsync(Arg.Any<INonQueryCommand>());
    }

    [Fact]
    public async Task VerifyApiKey_WithNoCachedResult_ProcessesNewVerification()
    {
        // Arrange
        var key = "testkey";
        var username = "testuser";
        var password = "testpass";
        var cachedKey = $"{key}_{username}";

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(cachedKey).Returns((VerifyExtApiKeyResult)null);
        mockDynamicRepository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(1));

        // Act
        var result = await service.VerifyApiKey(key, username, password);

        // Assert
        Assert.NotNull(result);
        await mockDynamicRepository.Received(1).ExecuteAsync(Arg.Any<INonQueryCommand>());
    }

    [Fact]
    public async Task VerifyApiKey_CallsCacheServiceWithCorrectKey()
    {
        // Arrange
        var key = "testkey";
        var username = "testuser";
        var password = "testpass";
        var expectedCachedKey = $"{key}_{username}";

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(expectedCachedKey).Returns((VerifyExtApiKeyResult)null);
        mockDynamicRepository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(1));

        // Act
        await service.VerifyApiKey(key, username, password);

        // Assert
        await mockCacheService.Received(1).GetAsync<VerifyExtApiKeyResult>(expectedCachedKey);
    }

    [Fact]
    public async Task VerifyApiKey_WithEmptyKey_ProcessesCorrectly()
    {
        // Arrange
        var key = "";
        var username = "testuser";
        var password = "testpass";

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(Arg.Any<string>()).Returns((VerifyExtApiKeyResult)null);
        mockDynamicRepository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(1));

        // Act
        var result = await service.VerifyApiKey(key, username, password);

        // Assert
        Assert.NotNull(result);
        await mockDynamicRepository.Received(1).ExecuteAsync(Arg.Any<INonQueryCommand>());
    }

    [Fact]
    public async Task VerifyApiKey_WithEmptyUsername_ProcessesCorrectly()
    {
        // Arrange
        var key = "testkey";
        var username = "";
        var password = "testpass";

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(Arg.Any<string>()).Returns((VerifyExtApiKeyResult)null);
        mockDynamicRepository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(1));

        // Act
        var result = await service.VerifyApiKey(key, username, password);

        // Assert
        Assert.NotNull(result);
        await mockDynamicRepository.Received(1).ExecuteAsync(Arg.Any<INonQueryCommand>());
    }

    [Fact]
    public async Task VerifyApiKey_WithEmptyPassword_ProcessesCorrectly()
    {
        // Arrange
        var key = "testkey";
        var username = "testuser";
        var password = "";

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(Arg.Any<string>()).Returns((VerifyExtApiKeyResult)null);
        mockDynamicRepository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(1));

        // Act
        var result = await service.VerifyApiKey(key, username, password);

        // Assert
        Assert.NotNull(result);
        await mockDynamicRepository.Received(1).ExecuteAsync(Arg.Any<INonQueryCommand>());
    }

    [Fact]
    public async Task VerifyApiKey_CreatesNewResultWhenNotCached()
    {
        // Arrange
        var key = "testkey";
        var username = "testuser";
        var password = "testpass";

        mockCacheService.GetAsync<VerifyExtApiKeyResult>(Arg.Any<string>()).Returns((VerifyExtApiKeyResult)null);
        mockDynamicRepository.ExecuteAsync(Arg.Any<INonQueryCommand>()).Returns(Task.FromResult(1));

        // Act
        var result = await service.VerifyApiKey(key, username, password);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<VerifyExtApiKeyResult>(result);
    }

    [Fact]
    public void Constructor_WithValidParameters_InitializesCorrectly()
    {
        // Arrange & Act
        var service = new ExtApiKeyService(mockDynamicRepository, mockCacheService);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullDynamicRepository_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new ExtApiKeyService(null, mockCacheService);
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_WithNullCacheService_DoesNotThrowException()
    {
        // Arrange & Act & Assert
        var service = new ExtApiKeyService(mockDynamicRepository, null);
        Assert.NotNull(service);
    }
}
