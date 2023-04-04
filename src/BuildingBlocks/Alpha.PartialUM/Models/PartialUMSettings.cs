namespace Alpha.PartialUM.Models;

public record PartialUMSettings()
{
    public string SiteGroup { get; init; }
    public int BetListsCacheInSeconds { get; init; }
    public IEnumerable<PartialUMFunctionSetting>? Functions { get; init; }
}