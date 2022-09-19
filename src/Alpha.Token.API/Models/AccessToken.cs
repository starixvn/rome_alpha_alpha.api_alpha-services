namespace Alpha.Token.API.Models;

public record AccessToken(int CustId, string ApiName, string ApiKey, string PassPhrase, string ApiSecret, string[] Scopes, DateTime? ExpirationDate)
{
    public DateTime DateCreated { get; } = DateTime.Now;

    public DateTime? LastUsed { get; set; } = default!;

    public bool IsExpired { get; } = ExpirationDate != null && ExpirationDate.Value < DateTime.Now;
}