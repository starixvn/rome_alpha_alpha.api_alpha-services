using System.Threading.RateLimiting;
using Alpha.Utilities.Configurations;

namespace Alpha.ApiGateways.Extensions
{
    public static class ConfigurationExtensions
    {
        public static PartitionedRateLimiter<HttpContext> CreateGlobalPartitionedRateLimiter(this IConfiguration configuration)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var partitionKey = "RateLimitByGlobalKey";
                var rateLimitConfig = configuration.GetSection(RateLimitConfig.ConfigByGlobal).Get<RateLimitConfig>();

                return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, partition => new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = rateLimitConfig.AutoReplenishment,
                    PermitLimit = rateLimitConfig.PermitLimit,
                    Window = rateLimitConfig.WindowByUnit,
                    SegmentsPerWindow = rateLimitConfig.SegmentsPerWindow
                });
            });
        }

        public static PartitionedRateLimiter<HttpContext> CreatePartitionedRateLimiter(this IConfiguration configuration, string rateLimiterName)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var headers = httpContext.Request.Headers;
                var partitionKey = headers.ContainsKey("Key") ? headers["Key"].ToString() : string.Empty;

                var rateLimitConfig = configuration.GetSection(rateLimiterName).Get<RateLimitConfig>();
                var message = rateLimitConfig.GetMessage(rateLimiterName, rateLimitConfig.PermitLimit);

                UpdateLastHitRateLimiterMessageToHeaders(message, httpContext);

                return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, partition => new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = rateLimitConfig.AutoReplenishment,
                    PermitLimit = rateLimitConfig.PermitLimit,
                    Window = rateLimitConfig.WindowByUnit,
                    SegmentsPerWindow = rateLimitConfig.SegmentsPerWindow
                });
            });
        }

        private static void UpdateLastHitRateLimiterMessageToHeaders(string rateLimiterName, HttpContext httpContext)
        {
            if (httpContext.Request.Headers.ContainsKey("RateLimiterMessage"))
                httpContext.Request.Headers.Remove("RateLimiterMessage");

            httpContext.Request.Headers.Add("RateLimiterMessage", rateLimiterName);
        }
    }
}