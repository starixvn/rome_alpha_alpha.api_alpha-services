namespace Alpha.ApiGateways.Models;

public class VerifyApiKeyResult
{
    public bool IsAuthenticated { get; set; }
    public string[] Scopes { get; set; } = Array.Empty<string>();
}
