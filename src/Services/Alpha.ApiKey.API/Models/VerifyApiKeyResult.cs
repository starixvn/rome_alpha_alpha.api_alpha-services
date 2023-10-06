namespace Alpha.ApiKey.API.Models;

public record VerifyApiKeyResult(bool IsAuthenticated, string ErrorMessage = "")
{
    public int CustId { get; init; } = default!;
    public int RoleId { get; init; } = default!;
    public string Scopes { get; init; } = default!;
}