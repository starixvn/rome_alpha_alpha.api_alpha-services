using System.Net;
using Alpha.ApiGateways.Configurations;
using Alpha.ApiGateways.Middlewares.RateLimit;
using Alpha.ApiGateways.Shared.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Alpha.ApiGateways.RateLimit.Middlewares;

public class RateLimitMiddleware
{
    private readonly RequestDelegate next;
    private readonly IDistributedCache cache;
    private readonly IConfiguration configuration;

    public RateLimitMiddleware(RequestDelegate next, IDistributedCache cache, IConfiguration configuration)
    {
        this.next = next;
        this.cache = cache;
        this.configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var rateLimit = configuration.GetSection("RateLimit").Get<LimitRequest>();

        if (rateLimit is null)
        {
            await next(context);
            return;
        }

        var key = GenerateClientKey(context);
        var requestStatistics = await GetRequestStatisticsByKey(key);

        if (requestStatistics != null && DateTime.UtcNow < requestStatistics.LastSuccessfulResponseTime.AddSeconds(rateLimit.TimeWindow) && requestStatistics.TotalRequestOfKey == rateLimit.MaxRequest)
        {
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            return;
        }

        await UpdateRequestStatisticsStorage(key, rateLimit.MaxRequest);
        await next(context);
    }

    private static string GenerateClientKey(HttpContext context) => $"{context.Request.Path}_{context.Request.Headers["Key"]}";

    private async Task<RequestStatistics?> GetRequestStatisticsByKey(string key) => await cache.GetCacheValueAsync<RequestStatistics>(key);

    private async Task UpdateRequestStatisticsStorage(string key, int maxRequests)
    {
        var requestStat = await cache.GetCacheValueAsync<RequestStatistics>(key);

        if (requestStat != null)
        {
            requestStat.LastSuccessfulResponseTime = DateTime.UtcNow;

            if (requestStat.TotalRequestOfKey == maxRequests)
                requestStat.TotalRequestOfKey = 1;

            else
                requestStat.TotalRequestOfKey++;

            await cache.SetCacheValueAsync(key, requestStat);
        }
        else
        {
            var clientStatistics = new RequestStatistics
            {
                LastSuccessfulResponseTime = DateTime.UtcNow,
                TotalRequestOfKey = 1
            };

            await cache.SetCacheValueAsync(key, clientStatistics);
        }
    }
}