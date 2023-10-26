using Alpha.Utilities.Extensions;

namespace Alpha.Utilities.Configurations;

public class RateLimitConfig
{
    public const string ConfigByGlobal = "RateLimitConfigs:RateLimitByGlobal";
    public const string ConfigBySecond = "RateLimitConfigs:RateLimitBySecond";
    public const string ConfigByMinute = "RateLimitConfigs:RateLimitByMinute";
    public const string ConfigByHour = "RateLimitConfigs:RateLimitByHour";

    public bool AutoReplenishment { get; set; }
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public string Unit { get; set; }
    public int SegmentsPerWindow { get; set; }
    public TimeSpan WindowByUnit => TimeSpanExtensions.ConvertWindowToTimeSpan(Window, Unit);

    public string GetMessage(string rateLimitConfig, int limit)
    {
        var message = $"Your Key reached limit {limit * 2} requests ";
        switch (rateLimitConfig)
        {
            case ConfigByGlobal: return $"System received too many requests";
            case ConfigBySecond: return $"{message}per second";
            case ConfigByMinute: return $"{message}per minute";
            case ConfigByHour: return $"{message}per hour";
            default: return "Unknown error";
        }
    }
    
}