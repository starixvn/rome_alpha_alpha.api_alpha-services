namespace Alpha.Token.API.Models;

public record VerifyTokenResult(bool IsAuthenticated)
{
    public string[] Scopes { get; init; } = default!;
}