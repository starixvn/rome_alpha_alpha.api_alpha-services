using System.Threading.RateLimiting;
using Alpha.Utilities.Configurations;

namespace Alpha.ApiGateways.Extensions
{
    public static class ConfigurationExtensions
    {
        public static PartitionedRateLimiter<HttpContext> CreateGlobalPartitionedRateLimiter(this IConfiguration configuration)
        {
            const string partitionKey = "RateLimitByGlobalKey";
            return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                RateLimitConfig rateLimitConfig = configuration.GetSection(RateLimitConfig.ConfigByGlobal).Get<RateLimitConfig>();
                httpContext.Request.Headers["RateLimiterMessage"] = rateLimitConfig.GetMessage(RateLimitConfig.ConfigByGlobal);

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
                string partitionKey = headers.ContainsKey("Key") ? headers["Key"].ToString() : string.Empty;

                RateLimitConfig rateLimitConfig = configuration.GetSection(rateLimiterName).Get<RateLimitConfig>();

                httpContext.Request.Headers["RateLimiterMessage"] = rateLimitConfig.GetMessage(rateLimiterName);

                return RateLimitPartition.GetSlidingWindowLimiter(partitionKey, partition => new SlidingWindowRateLimiterOptions
                {
                    AutoReplenishment = rateLimitConfig.AutoReplenishment,
                    PermitLimit = rateLimitConfig.PermitLimit,
                    Window = rateLimitConfig.WindowByUnit,
                    SegmentsPerWindow = rateLimitConfig.SegmentsPerWindow
                });
            });
        }
    }
}