namespace Alpha.PartialUM.Models;

public record PartialUMSettings
{
    public int PartialUMCacheInSeconds { get; init; }
    public IEnumerable<PartialUMFunctionSetting>? Functions { get; init; }
}