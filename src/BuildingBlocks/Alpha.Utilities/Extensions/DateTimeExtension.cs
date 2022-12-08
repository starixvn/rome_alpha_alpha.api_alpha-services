namespace Alpha.Utilities.Extensions;

public static class DateTimeExtension
{
    public static bool IsSameMonthWith(this DateTime dateTime, DateTime otherDateTime)
        => dateTime.Month == otherDateTime.Month && dateTime.Year == otherDateTime.Year;

    public static DateTime FirstDayOfYear(this DateTime dateTime)
        => new(dateTime.Year, 1, 1);

    public static DateTime FirstDayOfMonth(this DateTime dateTime)
        => new(dateTime.Year, dateTime.Month, 1);
}