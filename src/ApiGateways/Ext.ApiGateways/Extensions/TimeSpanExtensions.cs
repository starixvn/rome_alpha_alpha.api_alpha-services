namespace Ext.ApiGateways.Extensions;

public static class TimeSpanExtensions
{
    public static TimeSpan ConvertWindowToTimeSpan(int window, string unit)
        => unit?.ToLowerInvariant() switch
        {
            "d" => TimeSpan.FromDays(window),
            "h" => TimeSpan.FromHours(window),
            "m" => TimeSpan.FromMinutes(window),
            _ => TimeSpan.FromSeconds(window),
        };
}