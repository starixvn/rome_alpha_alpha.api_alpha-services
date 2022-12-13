namespace Alpha.ApiGateways.Shared.Extensions;

public static class TimeSpanExtensions
{
    public static TimeSpan ConvertWindowToTimeSpan(int window, string unit)
    {
        if (unit.Equals("d", StringComparison.OrdinalIgnoreCase))
        {
            return TimeSpan.FromDays(window);
        }
        if (unit.Equals("h", StringComparison.OrdinalIgnoreCase))
        {
            return TimeSpan.FromHours(window);
        }
        if (unit.Equals("m", StringComparison.OrdinalIgnoreCase))
        {
            return TimeSpan.FromMinutes(window);
        }

        return TimeSpan.FromSeconds(window);
    }
}