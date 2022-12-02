namespace Alpha.PartialUM.Models;

public record PartialUMFunctionSetting()
{
    public string? Name { get; init; }

    public IEnumerable<PartialUMSPSetting>? Settings { get; init; }
}