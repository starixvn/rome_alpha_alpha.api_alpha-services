namespace Alpha.ApiKey.API.Params;

public class CreateApiKeyParam
{
    public int CustId { get; set; }

    public byte RoleId { get; set; }

    public string ApiName { get; set; } = default!;

    public string PassPhrase { get; set; } = default!;

    public string[] Scopes { get; set; } = Array.Empty<string>();

    public DateTime? ExpirationDate { get; set; }

    public string RestrictIps { get; set; }
}