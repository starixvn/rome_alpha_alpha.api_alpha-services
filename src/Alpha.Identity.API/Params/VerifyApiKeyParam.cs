namespace Alpha.Identity.API.Params;

public class VerifyApiKeyParam
{
    public string Key { get; set; } = default!;

    public string PassPhrase { get; set; } = default!;

    public string Signature { get; set; } = default!;

    public string TimeSpan { get; set; } = default!;
}