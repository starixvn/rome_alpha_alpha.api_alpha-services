using Alpha.Utilities.Extensions;

namespace Ext.ApiGateways.Configurations;

public class CustomerRateLimitConfig
{
    public const string CustomerRateLimit = "RateLimitConfigs:CustomerRateLimit";
    public bool AutoReplenishment { get; set; }
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public string Unit { get; set; }
    public int SegmentsPerWindow { get; set; }
    public TimeSpan WindowByUnit => TimeSpanExtensions.ConvertWindowToTimeSpan(Window, Unit);
}