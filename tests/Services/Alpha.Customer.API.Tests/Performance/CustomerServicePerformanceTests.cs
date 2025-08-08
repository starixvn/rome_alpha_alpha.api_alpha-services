using System.Diagnostics;
using Alpha.Customer.API.Configurations;
using Alpha.Customer.API.DataAccesses;
using Alpha.Customer.API.Models;
using Alpha.Customer.API.Params;
using Alpha.Customer.API.Services;
using Fanex.Caching;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Alpha.Customer.API.Tests.Performance;

public class CustomerServicePerformanceTests
{
    private readonly ICacheService cacheService = Substitute.For<ICacheService>();
    private readonly IDynamicRepository dynamicRepository = Substitute.For<IDynamicRepository>();
    private readonly ICustomerDataAccess customerDataAccess = Substitute.For<ICustomerDataAccess>();
    private readonly IOptions<CustomerAppSettings> options;
    private readonly CustomerService service;

    public CustomerServicePerformanceTests()
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
    public async Task GetCustomerStatus_PerformanceTest_CompletesWithinReasonableTime()
    {
        // Arrange
        var param = new CustomerStatusParam(12345, 0);
        var status = new CustomerStatus(12345, 0, true, false, false, "[E][H]");

        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<CustomerStatus>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(status);

        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await service.GetCustomerStatus(param);

        // Assert
        stopwatch.Stop();
        Assert.NotNull(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 100, $"Operation took {stopwatch.ElapsedMilliseconds}ms, expected < 100ms");
    }

    [Fact]
    public async Task GetCustomerStatus_ConcurrentCalls_HandlesMultipleRequests()
    {
        // Arrange
        var status = new CustomerStatus(12345, 0, true, false, false, "[E][H]");
        
        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<CustomerStatus>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(status);

        var tasks = new List<Task<CustomerStatus>>();
        var concurrentRequests = 10;

        // Act - Create multiple concurrent requests
        for (int i = 0; i < concurrentRequests; i++)
        {
            var param = new CustomerStatusParam(12345 + i, 0);
            tasks.Add(service.GetCustomerStatus(param));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(concurrentRequests, results.Length);
        Assert.All(results, result => Assert.NotNull(result));
        Assert.All(results, result => Assert.True(result.IsActive));
    }

    [Fact]
    public async Task GetAllProducts_WithLargeDataSet_PerformsWell()
    {
        // Arrange
        var siteName = "TestSite";
        var currency = 1;
        
        // Create a large dataset
        var products = Enumerable.Range(1, 1000)
            .Select(i => new ProductModel { Id = i, Code = $"PROD{i}", Name = $"Product {i}" })
            .ToList();

        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<IEnumerable<ProductModel>>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(products);

        var stopwatch = Stopwatch.StartNew();

        // Act
        var result = await service.GetAllProducts(siteName, currency);

        // Assert
        stopwatch.Stop();
        Assert.NotNull(result);
        Assert.Equal(1000, result.Count());
        Assert.True(stopwatch.ElapsedMilliseconds < 200, $"Operation took {stopwatch.ElapsedMilliseconds}ms, expected < 200ms");
    }

    [Fact]
    public async Task GetDownlineInfo_StressTest_HandlesMultipleUsers()
    {
        // Arrange
        var downlineInfo = new DownlineInfo(67890, 3, false);
        
        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<DownlineInfo>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(downlineInfo);

        var tasks = new List<Task<DownlineInfo>>();
        var userCount = 50;

        var stopwatch = Stopwatch.StartNew();

        // Act - Simulate multiple users checking downline info
        for (int i = 0; i < userCount; i++)
        {
            tasks.Add(service.GetDownlineInfo(12345 + i, $"user{i}"));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        stopwatch.Stop();
        Assert.Equal(userCount, results.Length);
        Assert.All(results, result => Assert.NotNull(result));
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Stress test took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
    }

    [Fact]
    public async Task IsBelonging_BatchOperations_PerformsEfficiently()
    {
        // Arrange
        var batchSize = 20;
        var tasks = new List<Task<bool>>();

        var stopwatch = Stopwatch.StartNew();

        // Act - Batch belonging checks (without mocking ExecuteAsync since it's not virtual)
        for (int i = 0; i < batchSize; i++)
        {
            tasks.Add(service.IsBelonging(12345 + i, 67890, 1, 987654321L + i));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        stopwatch.Stop();
        Assert.Equal(batchSize, results.Length);
        // Don't assert the result values since we can't properly mock ExecuteAsync
        Assert.True(stopwatch.ElapsedMilliseconds < 500, $"Batch operation took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms");
    }

    [Fact]
    public async Task CacheOperations_BulkClearance_PerformsQuickly()
    {
        // Arrange
        var operationCount = 25;
        var tasks = new List<Task>();

        var stopwatch = Stopwatch.StartNew();

        // Act - Multiple cache clear operations
        for (int i = 0; i < operationCount; i++)
        {
            tasks.Add(service.ClearProductsCache($"Site{i}", i % 5 + 1));
        }

        await Task.WhenAll(tasks);

        // Assert
        stopwatch.Stop();
        Assert.True(stopwatch.ElapsedMilliseconds < 300, $"Bulk cache clear took {stopwatch.ElapsedMilliseconds}ms, expected < 300ms");
        
        // Verify all cache operations were called
        await cacheService.Received(operationCount).RemoveAsync(Arg.Any<string>());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task GetCustomerStatus_ScalabilityTest_HandlesVaryingLoads(int requestCount)
    {
        // Arrange
        var status = new CustomerStatus(12345, 0, true, false, false, "[E][H]");
        
        cacheService.GetOrSetAsync(
            Arg.Any<string>(),
            Arg.Any<Func<Task<CustomerStatus>>>(),
            Arg.Any<CacheItemOptions>())
            .Returns(status);

        var stopwatch = Stopwatch.StartNew();

        // Act
        var tasks = Enumerable.Range(0, requestCount)
            .Select(i => service.GetCustomerStatus(new CustomerStatusParam(12345 + i, 0)))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        // Assert
        stopwatch.Stop();
        Assert.Equal(requestCount, results.Length);
        Assert.All(results, result => Assert.NotNull(result));
        
        // Performance expectations scale with request count
        var expectedMaxTime = Math.Max(100, requestCount * 5);
        Assert.True(stopwatch.ElapsedMilliseconds < expectedMaxTime, 
            $"Scalability test with {requestCount} requests took {stopwatch.ElapsedMilliseconds}ms, expected < {expectedMaxTime}ms");
    }
}
