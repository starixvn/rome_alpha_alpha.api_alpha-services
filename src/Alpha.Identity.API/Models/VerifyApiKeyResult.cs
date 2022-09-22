namespace Alpha.Identity.API.Models;

public record VerifyApiKeyResult(bool IsAuthenticated)
{
    public string[] Scopes { get; init; } = default!;
}