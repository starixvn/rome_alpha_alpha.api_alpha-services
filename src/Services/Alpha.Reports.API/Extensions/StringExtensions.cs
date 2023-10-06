namespace Alpha.Reports.API.Extensions;

public static class StringExtensions
{
    public static List<int> ToListInt(this string delimitedString)
    {
        if (string.IsNullOrWhiteSpace(delimitedString))
        {
            return Enumerable.Empty<int>().ToList();
        }

        var ids = delimitedString
            .Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            .Where(str => int.TryParse(str, out _))
            .Select(str => Convert.ToInt32(str)).ToList();

        return ids;
    }
}