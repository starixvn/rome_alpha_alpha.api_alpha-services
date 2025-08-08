using Alpha.Customer.API.Configurations;
using Alpha.Customer.API.Criteria;
using Alpha.Customer.API.DataAccesses;
using Alpha.Customer.API.Models;
using Alpha.Customer.API.Params;
using Alpha.Customer.API.Services;
using Fanex.Caching;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Alpha.Customer.API.Tests.Integration;

public class CustomerServiceIntegrationTests
{
    private readonly ICacheService cacheService = Substitute.For<ICacheService>();
    private readonly IDynamicRepository dynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly ICustomerDataAccess customerDataAccess = Substitute.For<ICustomerDataAccess>();
    private readonly IOptions<CustomerAppSettings> options;
    private readonly CustomerService service;

    public CustomerServiceIntegrationTests()
    {
        var appSettings = new CustomerAppSettings
        {
            CustomerStatusCacheInSeconds = 300,
            ProductCacheInSeconds = 3600
        };
        options = Options.Create(appSettings);
        service = new CustomerService(cacheService, options, dynamicRepository, customerDataAccess);
    }

    [Fact]
    public async Task GetCustomerStatus_WithCacheHit_DoesNotHitDatabase()
    {
        // Arrange
        var param = new CustomerStatusParam(12345, 0);
        var cachedStatus = new CustomerStatus(12345, 0, true, false, false, "[E][H]");

        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<CustomerStatus>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(cachedStatus);

        // Act
        var result = await service.GetCustomerStatus(param);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cachedStatus.CustId, result.CustId);
        
        // Verify database was not called
        await dynamicRepository.DidNotReceive().FetchAsync<CustomerStatus>(Arg.Any<GetCustomerStatusCriteria>());
    }

    [Fact]
    public async Task GetCustomerStatus_WithCacheMiss_HitsDatabase()
    {
        // Arrange
        var param = new CustomerStatusParam(12345, 0);
        var dbStatus = new CustomerStatus(12345, 0, true, false, false, "[E][H]");
        var statusList = new List<CustomerStatus> { dbStatus };

        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<CustomerStatus>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(callInfo => callInfo.Arg<Func<Task<CustomerStatus>>>()());

        dynamicRepository.FetchAsync<CustomerStatus>(Arg.Any<GetCustomerStatusCriteria>())
            .Returns(statusList);

        // Act
        var result = await service.GetCustomerStatus(param);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dbStatus.CustId, result.CustId);
        
        // Verify database was called
        await dynamicRepository.Received(1).FetchAsync<CustomerStatus>(Arg.Any<GetCustomerStatusCriteria>());
    }

    [Fact]
    public async Task GetAllProducts_WithMultipleCalls_UsesCacheOnSecondCall()
    {
        // Arrange
        var siteName = "TestSite";
        var currency = 1;
        var products = new List<ProductModel>
        {
            new() { Id = 1, Code = "PROD1", Name = "Product 1" },
            new() { Id = 2, Code = "PROD2", Name = "Product 2" }
        };

        var callCount = 0;
        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<IEnumerable<ProductModel>>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(callInfo =>
            {
                callCount++;
                if (callCount == 1)
                {
                    // First call - cache miss, execute function
                    return callInfo.Arg<Func<Task<IEnumerable<ProductModel>>>>()();
                }
                else
                {
                    // Second call - cache hit, return cached value
                    return Task.FromResult<IEnumerable<ProductModel>>(products);
                }
            });

        dynamicRepository.FetchAsync<ProductModel>(Arg.Any<GetAllProductCriteria>())
            .Returns(products);

        // Act - First call
        var result1 = await service.GetAllProducts(siteName, currency);
        
        // Act - Second call
        var result2 = await service.GetAllProducts(siteName, currency);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(2, result1.Count());
        Assert.Equal(2, result2.Count());

        // Verify database was only called once (first call)
        await dynamicRepository.Received(1).FetchAsync<ProductModel>(Arg.Any<GetAllProductCriteria>());
    }

    [Fact]
    public async Task GetCustomerInfo_WithInvalidData_HandlesGracefully()
    {
        // Arrange
        var custId = 12345;
        var username = "testuser";

        customerDataAccess.GetCustomerInfo(Arg.Any<GetCustomerInfoCriteria>())
            .Returns(Task.FromException<CustomerInfoModel>(new Exception("Database connection failed")));

        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<CustomerInfoModel>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(callInfo => callInfo.Arg<Func<Task<CustomerInfoModel>>>()());

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => service.GetCustomerInfo(custId, username));
    }

    [Fact]
    public async Task ClearProductsCache_RemovesCorrectCacheKey()
    {
        // Arrange
        var siteName = "TestSite";
        var currency = 1;
        var expectedCacheKey = $"Alpha.Customer.API.Products_{siteName}_{currency}";

        // Act
        await service.ClearProductsCache(siteName, currency);

        // Assert
        await cacheService.Received(1).RemoveAsync(expectedCacheKey);
    }

    [Fact]
    public async Task MultipleServiceCalls_WithSameParameters_UseCacheEfficiently()
    {
        // Arrange
        var param = new CustomerStatusParam(12345, 0);
        var status = new CustomerStatus(12345, 0, true, false, false, "[E][H]");

        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<CustomerStatus>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(status);

        // Act - Multiple calls with same parameters
        var result1 = await service.GetCustomerStatus(param);
        var result2 = await service.GetCustomerStatus(param);
        var result3 = await service.GetCustomerStatus(param);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.NotNull(result3);
        Assert.All(new[] { result1, result2, result3 }, r => Assert.Equal(12345, r.CustId));

        // Verify cache was called 3 times but with same key
        await cacheService.Received(3).GetOrSetAsync(
            "Alpha.Customer.API.CustomerStatus.12345.0",
            Arg.Any<Func<Task<CustomerStatus>>>(),
            Arg.Any<CacheItemOptions>());
    }
}
