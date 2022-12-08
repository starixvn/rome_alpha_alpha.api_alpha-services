using System.Security.Claims;
using System.Threading.RateLimiting;
using Alpha.ApiGateways.Configurations;
using Microsoft.AspNetCore.RateLimiting;

namespace Alpha.ApiGateways.Infrastructure.RateLimits;

public static class CustomRateLimitOptions
{
    public static RateLimiterOptions GetIpRateLimiterOptions(IConfiguration configuration)
    {
        var customerRateLimitConfig = new CustomerRateLimitConfig();
        configuration.GetSection(CustomerRateLimitConfig.CustomerRateLimit).Bind(customerRateLimitConfig);

        return new RateLimiterOptions
        {
            RejectionStatusCode = StatusCodes.Status429TooManyRequests,
            GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    string remoteIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: remoteIpAddress,
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = customerRateLimitConfig.AutoReplenishment,
                            PermitLimit = customerRateLimitConfig.PermitLimit,
                            Window = TimeSpan.FromMinutes(customerRateLimitConfig.Window)
                        }
                    );
                })
            )
        };
    }

    public static RateLimiterOptions GetCustIdRateLimiterOptions(IConfiguration configuration)
    {
        var ipRateLimitConfig = new IPRateLimitConfig();
        configuration.GetSection(IPRateLimitConfig.IpRateLimit).Bind(ipRateLimitConfig);

        return new RateLimiterOptions
        {
            RejectionStatusCode = StatusCodes.Status429TooManyRequests,
            GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = ipRateLimitConfig.AutoReplenishment,
                            PermitLimit = ipRateLimitConfig.PermitLimit,
                            Window = TimeSpan.FromMinutes(ipRateLimitConfig.Window)
                        }
                    );
                })
            )
        };
    }
}