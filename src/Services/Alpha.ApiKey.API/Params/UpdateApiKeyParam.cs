namespace Alpha.ApiKey.API.Params;

public class UpdateApiKeyParam
{
    public string ApiKey { get; set; }

    public int CustId { get; set; }

    public string ApiName { get; set; } = default!;

    public string Scopes { get; set; } = default!;

    public string IP { get; set; } = default!;

    public string Language { get; set; } = default!;

    public DateTime? ExpirationDate { get; set; }
}