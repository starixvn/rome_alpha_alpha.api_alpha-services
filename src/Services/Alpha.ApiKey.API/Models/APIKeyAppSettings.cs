namespace Alpha.ApiKey.API.Models;

public record APIKeyAppSettings()
{
    public int ExpireSignatureTime { get; init; }
}