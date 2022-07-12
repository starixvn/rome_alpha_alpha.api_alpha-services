namespace Alpha.BetList.API.Models;

public class VerifyTokenResult
{
    public bool IsAuthenticated { get; set; }

    public string[] Scopes { get; set; } = Array.Empty<string>();
}