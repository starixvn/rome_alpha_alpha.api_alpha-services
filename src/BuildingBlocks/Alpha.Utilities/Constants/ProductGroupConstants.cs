namespace Alpha.Utilities.Constants;

public static class ProductGroupConstants
{
    public const string All = "All";
    public const string Sportsbook = "Sportsbook";
    public const string Casino = "Casino";
    private static readonly string[] ValidGroups = new[] { All, Sportsbook, Casino };

    public static IReadOnlyList<string> Groups => ValidGroups;

    public static bool IsValid(string name)
        => ValidGroups.Any(g => string.Equals(name, g, StringComparison.OrdinalIgnoreCase));
}