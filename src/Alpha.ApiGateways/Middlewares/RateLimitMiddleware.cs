using System.Net;
using Alpha.ApiGateways.Configurations;
using Alpha.ApiGateways.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Alpha.ApiGateways.Middlewares
{
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
            var rateLimitingAttribute = configuration.GetSection("RateLimit").Get<LimitRequest>();

            if (rateLimitingAttribute is null)
            {
                await next(context);
                return;
            }

            var key = GenerateClientKey(context);
            var requestStatistics = await GetRequestStatisticsByKey(key);

            if (requestStatistics != null && DateTime.UtcNow < requestStatistics.LastSuccessfulResponseTime.AddSeconds(rateLimitingAttribute.TimeWindow) && requestStatistics.NumberOfRequestsCompletedSuccessfully == rateLimitingAttribute.MaxRequest)
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            await UpdateRequestStatisticsStorage(key, rateLimitingAttribute.MaxRequest);
            await next(context);
        }

        private static string GenerateClientKey(HttpContext context) => $"{context.Request.Path}_{context.Request.Headers["token"]}";

        private async Task<RequestStatistics?> GetRequestStatisticsByKey(string key) => await cache.GetCacheValueAsync<RequestStatistics>(key);

        private async Task UpdateRequestStatisticsStorage(string key, int maxRequests)
        {
            var requestStat = await cache.GetCacheValueAsync<RequestStatistics>(key);

            if (requestStat != null)
            {
                requestStat.LastSuccessfulResponseTime = DateTime.UtcNow;

                if (requestStat.NumberOfRequestsCompletedSuccessfully == maxRequests)
                    requestStat.NumberOfRequestsCompletedSuccessfully = 1;

                else
                    requestStat.NumberOfRequestsCompletedSuccessfully++;

                await cache.SetCacheValueAsync(key, requestStat);
            }
            else
            {
                var clientStatistics = new RequestStatistics
                {
                    LastSuccessfulResponseTime = DateTime.UtcNow,
                    NumberOfRequestsCompletedSuccessfully = 1
                };

                await cache.SetCacheValueAsync(key, clientStatistics);
            }
        }
    }

    public class RequestStatistics
    {
        public DateTime LastSuccessfulResponseTime { get; set; }
        public int NumberOfRequestsCompletedSuccessfully { get; set; }
    }
}