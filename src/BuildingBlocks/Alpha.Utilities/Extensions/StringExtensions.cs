using System.Globalization;

namespace Alpha.Utilities.Extensions;

public static class StringExtensions
{
    public const string NotAvailableNumber = "N/A";

    public static string FormatNumber(this decimal numberValue)
    {
        var result = numberValue.ToString("#,##0.00", CultureInfo.InvariantCulture);

        return numberValue < 0 && !result.StartsWith("-", StringComparison.OrdinalIgnoreCase)
            ? $"-{result}"
            : result;
    }

    public static string FormatNumber(this decimal? numberValue)
        => numberValue.HasValue ? FormatNumber(numberValue.Value) : NotAvailableNumber;

    public static List<int> ToListInt(this string delimitedString)
    {
        if (string.IsNullOrWhiteSpace(delimitedString))
        {
            return Enumerable.Empty<int>().ToList();
        }

        var ids = delimitedString
            .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            .Where(str => int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
            .Select(str => Convert.ToInt32(str, CultureInfo.InvariantCulture)).ToList();

        return ids;
    }

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