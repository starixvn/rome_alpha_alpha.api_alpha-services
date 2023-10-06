namespace Alpha.ApiGateways.Models;

public class VerifyApiKeyResult
{
    public bool IsAuthenticated { get; set; }
    public int CustId { get; set; }
    public int RoleId { get; set; }
    public string ErrorMessage { get; set; }
    public string[] Scopes { get; set; } = Array.Empty<string>();
}
