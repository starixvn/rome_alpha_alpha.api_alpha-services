using Alpha.ApiGateways.Shared.Extensions;

namespace Alpha.ApiGateways.Configurations;

public class IPRateLimitConfig
{
    public const string IpRateLimit = "RateLimitConfigs:IpRateLimit";
    public bool AutoReplenishment { get; set; }
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public string Unit { get; set; }
    public int SegmentsPerWindow { get; set; }
    public TimeSpan WindowByUnit => TimeSpanExtensions.ConvertWindowToTimeSpan(Window, Unit);
}