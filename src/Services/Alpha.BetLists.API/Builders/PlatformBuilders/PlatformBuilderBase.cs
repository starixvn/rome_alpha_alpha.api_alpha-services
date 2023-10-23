public static class PlatformBuilderBase
{
    private static readonly Dictionary<int, string> PlatformInfo = new()
    {
      [0] = "N/A",
      [1] = "Desktop",
      [2] = "Mobile",
      [3] = "APP",
      [4] = "Uncategorized",
      [5] = "Uncategorized"
    };

    public static string BuildPlatform(int? deviceType)
    {
      var platform = deviceType ?? 0;

      return PlatformInfo.ContainsKey(platform) ? PlatformInfo[platform] : PlatformInfo[5];
    }

}