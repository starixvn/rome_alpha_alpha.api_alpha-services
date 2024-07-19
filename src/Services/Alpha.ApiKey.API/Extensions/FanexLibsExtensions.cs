using Fanex.Caching;

namespace Alpha.ApiKey.API.Extensions;

public static class FanexLibsExtensions
{
    public static IServiceCollection AddFanexCachingMemory(this IServiceCollection services)
        => services.AddSingleton<ICacheService, CacheService>();
}