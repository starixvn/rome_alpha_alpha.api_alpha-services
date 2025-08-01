namespace Alpha.BetLists.API.Builders.PlatformBuilders;
public static class PlatformBuilderBase
{
    private static readonly Dictionary<int, string> PlatformInfo = new()
    {
      [0] = NotAvailable,
      [1] = "Desktop",
      [2] = "Mobile",
      [3] = "APP",
      [99] = Uncategorized
    };

    private const string NotAvailable = "N/A";
    private const string Uncategorized = "Uncategorized";

    public static string BuildPlatform(int? deviceType)
    {
        if (deviceType == null)
        {
            return NotAvailable;
        }

        return PlatformInfo.ContainsKey((int)deviceType) ? PlatformInfo[(int)deviceType] : Uncategorized;
    }

}