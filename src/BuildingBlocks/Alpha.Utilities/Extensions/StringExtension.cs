namespace Alpha.Utilities.Extensions;

public static class StringExtension
{
    public static string FormatNumber(this decimal numberValue)
    {
        var result = numberValue.ToString("#,##0.00");

        return numberValue < 0 && !result.StartsWith("-", StringComparison.OrdinalIgnoreCase)
            ? $"-{result}"
            : result;
    }
}