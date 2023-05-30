namespace Alpha.PartialUM.Models;

public record PartialUMSettings()
{
    public int BetListsCacheInSeconds { get; init; }
    public IEnumerable<PartialUMFunctionSetting>? Functions { get; init; }
}