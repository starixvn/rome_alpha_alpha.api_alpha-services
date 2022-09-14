namespace Alpha.Token.Alpha.Params;

public class VerifyTokenParam
{
    public string ApiKey { get; set; } = default!;

    public string PassPhrase { get; set; } = default!;

    public string ApiSign { get; set; } = default!;

    public string TimeSpan { get; set; } = default!;
}