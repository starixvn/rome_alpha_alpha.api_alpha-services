using System.Security.Claims;
using System.Threading.RateLimiting;
using Ext.ApiGateways.Configurations;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Ext.ApiGateways.Infrastructure.RateLimits;

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
        var custId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        var subAccId = httpContext.User.FindFirst("SubAccId")?.Value ?? string.Empty;

        return RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey: $"{custId}_{subAccId}",
            partition => new SlidingWindowRateLimiterOptions
            {
                AutoReplenishment = customerRateLimitConfig.AutoReplenishment,
                PermitLimit = customerRateLimitConfig.PermitLimit,
                Window = customerRateLimitConfig.WindowByUnit,
                SegmentsPerWindow = customerRateLimitConfig.SegmentsPerWindow
            });
    }
}