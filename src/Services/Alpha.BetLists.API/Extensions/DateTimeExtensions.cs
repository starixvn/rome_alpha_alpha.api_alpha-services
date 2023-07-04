namespace Alpha.BetLists.API.Extensions;

public static class DateTimeExtensions
{
    public static DateTime ToStartOfDay(this DateTime dateTime)
        => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);

    public static DateTime ToEndOfDay(this DateTime dateTime)
        => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);

    public static bool IsBefore(this DateTime dateTime, DateTime otherDateTime)
        => dateTime < otherDateTime;

    public static bool IsBeforeOrEqual(this DateTime dateTime, DateTime otherDateTime)
        => dateTime <= otherDateTime;
}