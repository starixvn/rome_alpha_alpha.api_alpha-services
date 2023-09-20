using System.Security.Claims;
using System.Threading.RateLimiting;
using Alpha.Utilities.Configurations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Alpha.ApiGateways.Infrastructure.RateLimits;

public class CustomRateLimiterPolicy : IRateLimiterPolicy<string>
{
    private readonly Func<OnRejectedContext, CancellationToken, ValueTask> _onRejected;
    private readonly CustomerRateLimitConfig customerRateLimitConfig;

    public CustomRateLimiterPolicy(IOptions<CustomerRateLimitConfig> options)
    {
        _onRejected = (ctx, token) =>
        {
            ctx.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            return ValueTask.CompletedTask;
        };
        customerRateLimitConfig = options.Value;
    }

    public Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected => _onRejected;

    public RateLimitPartition<string> GetPartition(HttpContext httpContext)
    {
        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            partition => new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = customerRateLimitConfig.AutoReplenishment,
                PermitLimit = customerRateLimitConfig.PermitLimit,
                Window = customerRateLimitConfig.WindowByUnit,
                SegmentsPerWindow = customerRateLimitConfig.SegmentsPerWindow
            });
    }
}