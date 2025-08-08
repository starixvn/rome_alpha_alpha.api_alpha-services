using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alpha.ApiKey.API.Controller;
using Alpha.ApiKey.API.Models;
using Alpha.ApiKey.API.Params;
using Alpha.ApiKey.API.Services;
using Alpha.ApiKey.API.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Alpha.ApiKey.API.Tests.Controllers;

public class ApiKeyControllerTests
{
    private readonly IApiKeyService apiKeyService = Substitute.For<IApiKeyService>();
    private readonly ApiKeyController controller;

    public ApiKeyControllerTests()
    {
        controller = new ApiKeyController(apiKeyService);
    }

    [Fact]
    public async Task GetApiKeysByCustId_WithValidCustId_ReturnsOkWithApiKeys()
    {
        // Arrange
        var custId = 12345;
        var expectedApiKeys = new List<ApiKeyEntity>
        {
            TestDataFixtures.ApiKeyEntities.ValidApiKey,
            TestDataFixtures.ApiKeyEntities.ExpiredApiKey
        };

        apiKeyService.GetApiKeys(custId).Returns(expectedApiKeys);

        // Act
        var result = await controller.GetApiKeysByCustId(custId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedApiKeys = Assert.IsAssignableFrom<IEnumerable<ApiKeyEntity>>(okResult.Value);
        Assert.Equal(2, returnedApiKeys.Count());
    }

    [Fact]
    public async Task GetApiKeysByCustId_WithNonExistentCustId_ReturnsOkWithEmptyList()
    {
        // Arrange
        var custId = 99999;
        var emptyList = new List<ApiKeyEntity>();

        apiKeyService.GetApiKeys(custId).Returns(emptyList);

        // Act
        var result = await controller.GetApiKeysByCustId(custId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedApiKeys = Assert.IsAssignableFrom<IEnumerable<ApiKeyEntity>>(okResult.Value);
        Assert.Empty(returnedApiKeys);
    }

    [Fact]
    public async Task GetApiKeysByCustId_CallsServiceWithCorrectParameter()
    {
        // Arrange
        var custId = 12345;

        // Act
        await controller.GetApiKeysByCustId(custId);

        // Assert
        await apiKeyService.Received(1).GetApiKeys(custId);
    }

    [Fact]
    public async Task GetApiKey_WithValidKey_ReturnsOkWithApiKey()
    {
        // Arrange
        var key = "testkey123";
        var expectedApiKey = TestDataFixtures.ApiKeyEntities.ValidApiKey;

        apiKeyService.Get(key).Returns(expectedApiKey);

        // Act
        var result = await controller.GetApiKey(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedApiKey = Assert.IsType<ApiKeyEntity>(okResult.Value);
        Assert.Equal(expectedApiKey.Key, returnedApiKey.Key);
    }

    [Fact]
    public async Task GetApiKey_WithNonExistentKey_ReturnsOkWithNull()
    {
        // Arrange
        var key = "nonexistentkey";

        apiKeyService.Get(key).Returns((ApiKeyEntity)null);

        // Act
        var result = await controller.GetApiKey(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Null(okResult.Value);
    }

    [Fact]
    public async Task GetApiKey_CallsServiceWithCorrectParameter()
    {
        // Arrange
        var key = "testkey123";

        // Act
        await controller.GetApiKey(key);

        // Assert
        await apiKeyService.Received(1).Get(key);
    }

    [Fact]
    public async Task CreateApiKey_WithValidParam_ReturnsOkWithResult()
    {
        // Arrange
        var param = new CreateApiKeyParam
        {
            CustId = 12345,
            ApiName = "Test API",
            PassPhrase = "testpass",
            Scopes = "1,2,3",
            IP = "192.168.1.1",
            Language = "EN",
            ExpirationDate = DateTime.Now.AddDays(30)
        };

        var expectedResult = TestDataFixtures.CreateApiKeyResults.SuccessResult;
        apiKeyService.CreateApiKey(
            param.CustId, 
            param.ApiName, 
            param.PassPhrase, 
            param.Scopes, 
            param.IP, 
            param.Language, 
            param.ExpirationDate)
            .Returns(expectedResult);

        // Act
        var result = await controller.CreateApiKey(param);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResult = Assert.IsType<CreateApiKeyResult>(okResult.Value);
        Assert.NotNull(returnedResult.ApiKeyResult);
        Assert.Null(returnedResult.Result);
    }

    [Fact]
    public async Task CreateApiKey_WithError_ReturnsOkWithErrorResult()
    {
        // Arrange
        var param = new CreateApiKeyParam
        {
            CustId = 12345,
            ApiName = "Test API",
            PassPhrase = "testpass",
            Scopes = "1,2,3",
            IP = "192.168.1.1",
            Language = "EN",
            ExpirationDate = DateTime.Now.AddDays(30)
        };

        var expectedResult = TestDataFixtures.CreateApiKeyResults.ErrorResult;
        apiKeyService.CreateApiKey(
            param.CustId, 
            param.ApiName, 
            param.PassPhrase, 
            param.Scopes, 
            param.IP, 
            param.Language, 
            param.ExpirationDate)
            .Returns(expectedResult);

        // Act
        var result = await controller.CreateApiKey(param);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResult = Assert.IsType<CreateApiKeyResult>(okResult.Value);
        Assert.Null(returnedResult.ApiKeyResult);
        Assert.NotNull(returnedResult.Result);
        Assert.Equal(1001, returnedResult.Result.Code);
    }

    [Fact]
    public async Task CreateApiKey_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var param = new CreateApiKeyParam
        {
            CustId = 12345,
            ApiName = "Test API",
            PassPhrase = "testpass",
            Scopes = "1,2,3",
            IP = "192.168.1.1",
            Language = "EN",
            ExpirationDate = DateTime.Now.AddDays(30)
        };

        // Act
        await controller.CreateApiKey(param);

        // Assert
        await apiKeyService.Received(1).CreateApiKey(
            param.CustId,
            param.ApiName,
            param.PassPhrase,
            param.Scopes,
            param.IP,
            param.Language,
            param.ExpirationDate);
    }

    [Fact]
    public async Task UpdateApiKey_WithExistingKey_ReturnsOkWithResult()
    {
        // Arrange
        var param = new UpdateApiKeyParam
        {
            ApiKey = "testkey123",
            CustId = 12345,
            ApiName = "Updated API",
            Scopes = "1,2,3,4",
            IP = "192.168.1.2",
            Language = "EN",
            ExpirationDate = DateTime.Now.AddDays(60)
        };

        var existingApiKey = TestDataFixtures.ApiKeyEntities.ValidApiKey;
        var updateResult = new Result(0, "");

        apiKeyService.Get(param.ApiKey).Returns(existingApiKey);
        apiKeyService.UpdateApiKey(
            param.CustId,
            param.ApiKey,
            param.ApiName,
            param.Scopes,
            param.IP,
            param.Language,
            param.ExpirationDate)
            .Returns(updateResult);

        // Act
        var result = await controller.UpdateApiKey(param);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResult = Assert.IsType<Result>(okResult.Value);
        Assert.Equal(0, returnedResult.Code);
    }

    [Fact]
    public async Task UpdateApiKey_WithNonExistentKey_ReturnsNotFound()
    {
        // Arrange
        var param = new UpdateApiKeyParam
        {
            ApiKey = "nonexistentkey",
            CustId = 12345,
            ApiName = "Updated API",
            Scopes = "1,2,3,4",
            IP = "192.168.1.2",
            Language = "EN",
            ExpirationDate = DateTime.Now.AddDays(60)
        };

        apiKeyService.Get(param.ApiKey).Returns((ApiKeyEntity)null);

        // Act
        var result = await controller.UpdateApiKey(param);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateApiKey_WithDifferentCustId_ReturnsBadRequest()
    {
        // Arrange
        var param = new UpdateApiKeyParam
        {
            ApiKey = "testkey123",
            CustId = 99999, // Different from the API key's CustId
            ApiName = "Updated API",
            Scopes = "1,2,3,4",
            IP = "192.168.1.2",
            Language = "EN",
            ExpirationDate = DateTime.Now.AddDays(60)
        };

        var existingApiKey = TestDataFixtures.ApiKeyEntities.ValidApiKey; // CustId = 12345

        apiKeyService.Get(param.ApiKey).Returns(existingApiKey);

        // Act
        var result = await controller.UpdateApiKey(param);

        // Assert
        Assert.IsType<BadRequestResult>(result.Result);
    }

    [Fact]
    public async Task DeleteApiKey_WithExistingKey_ReturnsNoContent()
    {
        // Arrange
        var key = "testkey123";
        var ip = "192.168.1.1";

        var existingApiKey = TestDataFixtures.ApiKeyEntities.ValidApiKey;
        apiKeyService.Get(key).Returns(existingApiKey);

        // Act
        var result = await controller.DeleteApiKey(key, ip);

        // Assert
        Assert.IsType<NoContentResult>(result);
        await apiKeyService.Received(1).RevokeApiKey(key, existingApiKey.CustId, ip);
    }

    [Fact]
    public async Task DeleteApiKey_WithNonExistentKey_ReturnsNotFound()
    {
        // Arrange
        var key = "nonexistentkey";
        var ip = "192.168.1.1";

        apiKeyService.Get(key).Returns((ApiKeyEntity)null);

        // Act
        var result = await controller.DeleteApiKey(key, ip);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void Constructor_WithValidService_InitializesCorrectly()
    {
        // Act & Assert
        Assert.NotNull(controller);
    }
}
