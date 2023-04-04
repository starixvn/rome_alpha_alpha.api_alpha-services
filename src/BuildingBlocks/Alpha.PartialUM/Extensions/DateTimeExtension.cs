namespace Alpha.PartialUM.Extensions;

public static class DateTimeExtension
{
    public static string ToLongString(this DateTime dateTime)
        => dateTime.ToString("dd/MM/yyyy hh:mm tt");
}