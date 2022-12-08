using System.Reflection.Metadata;
namespace Alpha.ApiGateways.Configurations;

public class CustomerRateLimitConfig
{
    public const string CustomerRateLimit = "RateLimitConfigs:CustomerRateLimit";
    public bool AutoReplenishment { get; set; }
    public int PermitLimit { get; set; }
    public int Window { get; set; }
}