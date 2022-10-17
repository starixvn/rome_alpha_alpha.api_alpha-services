namespace Alpha.ApiKey.API.Models;

public record VerifyApiKeyResult(bool IsAuthenticated)
{
    public int CustId { get; init; } = default!;
    public int RoleId { get; init; } = default!;
}