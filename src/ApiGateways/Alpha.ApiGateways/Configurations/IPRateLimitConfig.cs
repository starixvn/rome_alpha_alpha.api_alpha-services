namespace Alpha.ApiGateways.Configurations;

public class IPRateLimitConfig
{
    public const string IpRateLimit = "RateLimitConfigs:IpRateLimit";
    public bool AutoReplenishment { get; set; }
    public int PermitLimit { get; set; }
    public int Window { get; set; }
}