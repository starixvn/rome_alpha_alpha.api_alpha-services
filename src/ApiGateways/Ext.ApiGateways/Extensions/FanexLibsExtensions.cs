using Fanex.Caching;

namespace Ext.ApiGateways.Extensions;

public static class FanexLibsExtensions
{
    public static IServiceCollection AddFanexCachingMemory(this IServiceCollection services)
        => services.AddSingleton<ICacheService, CacheService>();
}