namespace Alpha.Token.API.Models;

public record AccessToken(int CustId, string Name, string Token, string[] Scopes, DateTime? ExpirationDate)
{
    public DateTime DateCreated { get; } = DateTime.Now;

    public DateTime? LastUsed { get; set; } = default!;
}