namespace Alpha.Utilities.Constants;

public static class ProductGroupConstants
{
    public const string All = "All";
    public const string Sportsbook = "Sportsbook";
    public const string Casino = "Casino";

    public static bool IsValid(string name)
    {
        var supportedGroup = new[] { All, Sportsbook, Casino };

        return supportedGroup.Any(g => string.Equals(name, g, StringComparison.OrdinalIgnoreCase));
    }
}