namespace Alpha.Token.Alpha.Params;

public class CreateTokenParam
{
    public int CustId { get; set; }

    public string TokenName { get; set; } = default!;

    public string PassPhrase { get; set; } = default!;

    public string[] Scopes { get; set; } = Array.Empty<string>();

    public DateTime? ExpirationDate { get; set; }
}