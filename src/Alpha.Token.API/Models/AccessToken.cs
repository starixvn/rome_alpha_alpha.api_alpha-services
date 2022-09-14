namespace Alpha.Token.API.Models;

public record AccessToken(int CustId, string Name, string ApiKey, string PassPhrase, string Token, string[] Scopes, DateTime? ExpirationDate)
{
    public DateTime DateCreated { get; } = DateTime.Now;

    public DateTime? LastUsed { get; set; } = default!;

    public bool IsExpired { get; } = ExpirationDate != null && ExpirationDate.Value < DateTime.Now;
}