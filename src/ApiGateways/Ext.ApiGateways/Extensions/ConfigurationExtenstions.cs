using System.Threading.RateLimiting;
using Alpha.Utilities.Configurations;

namespace Ext.ApiGateways.Extensions
{
    public static class ConfigurationExtensions
    {
        public static PartitionedRateLimiter<HttpContext> CreatePartitionedRateLimiter(this IConfiguration configuration, string rateLimiterName)
        {
            return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                var headers = httpContext.Request.Headers;
                var partitionKey = headers.ContainsKey("Key") ? headers["Key"].ToString() : string.Empty;

                var rateLimitConfig = configuration.GetSection(rateLimiterName).Get<RateLimitConfig>();

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