using Alpha.Utilities.Extensions;

namespace Alpha.Utilities.Configurations;

public class RateLimitConfig
{
    public const string ConfigBySecond = "RateLimitConfigs:RateLimitBySecond";
    public const string ConfigByMinute = "RateLimitConfigs:RateLimitByMinute";
    public const string ConfigByHour = "RateLimitConfigs:RateLimitByHour";

    public bool AutoReplenishment { get; set; }
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public string Unit { get; set; }
    public int SegmentsPerWindow { get; set; }
    public TimeSpan WindowByUnit => TimeSpanExtensions.ConvertWindowToTimeSpan(Window, Unit);
}