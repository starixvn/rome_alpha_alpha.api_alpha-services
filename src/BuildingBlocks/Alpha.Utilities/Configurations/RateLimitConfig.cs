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

    public string GetMessage(string rateLimitConfig)
    {
        var message = $"Your Key reached limit {PermitLimit * 2} requests ";
        return rateLimitConfig switch
        {
            ConfigByGlobal => $"System received too many requests",
            ConfigBySecond => $"{message}per second",
            ConfigByMinute => $"{message}per minute",
            ConfigByHour => $"{message}per hour",
            _ => "Unknown error"
        };
    }
}