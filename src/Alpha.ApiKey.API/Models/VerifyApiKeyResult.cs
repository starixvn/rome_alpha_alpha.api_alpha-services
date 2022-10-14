namespace Alpha.ApiKey.Api.Models;

public record VerifyApiKeyResult(bool IsAuthenticated)
{
    public string[] Scopes { get; init; } = default!;
}