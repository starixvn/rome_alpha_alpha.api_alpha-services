namespace Alpha.Utilities.Extensions;

public static class StringExtensions
{
    public static string NotAvailableNumber = "N/A";

    public static string FormatNumber(this decimal numberValue)
    {
        var result = numberValue.ToString("#,##0.00");

        return numberValue < 0 && !result.StartsWith("-", StringComparison.OrdinalIgnoreCase)
            ? $"-{result}"
            : result;
    }

    public static string FormatNumber(this decimal? numberValue)
        => numberValue.HasValue ? FormatNumber(numberValue.Value) : NotAvailableNumber;

    public static List<int> ToListInt(this string delimitedString)
        => ToListString(delimitedString)
        .Where(str => int.TryParse(str, out _))
        .Select(str => Convert.ToInt32(str))
        .ToList();

    public static IEnumerable<string> ToListString(this string delimitedString)
        => string.IsNullOrWhiteSpace(delimitedString)
        ? new List<string>()
        : delimitedString.Split(',', StringSplitOptions.RemoveEmptyEntries);

    public static string CorrectDateTimeSpace(this string dateTime)
    {
        var suffix = new[] { "AM", "PM", "am", "pm" };
        var result = dateTime;

        foreach (var s in suffix)
        {
            var expectedS = " " + s;
            if (result.IndexOf(expectedS) == -1)
            {
                result = result.Replace(s, expectedS);
            }
        }

        return result;
    }
}