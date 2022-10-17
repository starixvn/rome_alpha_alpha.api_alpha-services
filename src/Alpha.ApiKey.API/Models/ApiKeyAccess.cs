namespace Alpha.ApiKey.API.Models;

public record ApiKeyAccess(int CustId, string ApiName, string Key, string PassPhrase, string ApiSecret, string[] Scopes, DateTime? ExpirationDate)
{
    public DateTime DateCreated { get; } = DateTime.Now;

    public DateTime? LastUsed { get; set; } = default!;

    public bool IsExpired { get; } = ExpirationDate != null && ExpirationDate.Value < DateTime.Now;
}