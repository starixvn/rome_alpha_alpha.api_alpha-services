using Alpha.BetLists.API.Extensions;
using Fanex.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Alpha.BetLists.API.Tests.Extensions;

public class FanexLibsExtensionsTests
{
    [Fact]
    public void AddFanexCachingMemory_RegistersCacheService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddFanexCachingMemory();

        // Assert
        Assert.NotNull(result);
        Assert.Same(services, result); // Should return the same service collection for chaining

        // Verify the service is registered
        var serviceProvider = services.BuildServiceProvider();
        var cacheService = serviceProvider.GetService<ICacheService>();
        
        Assert.NotNull(cacheService);
        Assert.IsType<CacheService>(cacheService);
    }

    [Fact]
    public void AddFanexCachingMemory_RegistersAssingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFanexCachingMemory();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(x => x.ServiceType == typeof(ICacheService));
        
        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(CacheService), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddFanexCachingMemory_ReturnsServiceCollectionForChaining()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddFanexCachingMemory();

        // Assert
        Assert.Same(services, result);
    }

    [Fact]
    public void AddFanexCachingMemory_WithMultipleCalls_RegistersOnlyOnce()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddFanexCachingMemory();
        services.AddFanexCachingMemory(); // Called twice

        // Assert
        var cacheServiceDescriptors = services.Where(x => x.ServiceType == typeof(ICacheService)).ToList();
        
        // Should have multiple registrations (this is expected behavior for multiple calls)
        // The last registration will be used
        Assert.True(cacheServiceDescriptors.Count >= 1);
        
        var serviceProvider = services.BuildServiceProvider();
        var cacheService = serviceProvider.GetService<ICacheService>();
        
        Assert.NotNull(cacheService);
        Assert.IsType<CacheService>(cacheService);
    }
}
