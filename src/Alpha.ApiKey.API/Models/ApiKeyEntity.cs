namespace Alpha.ApiKey.API.Models;

public record ApiKeyEntity()
{
    public int CustId { get; init; }

    public string Name { get; init; }

    public string Key { get; init; }

    public string Secret { get; init; }

    public string PassPhrase { get; init; }

    public string ScopIdList { get; init; }

    public int[] ScopIds => ScopIdList.Split(',')?.Select(int.Parse)?.ToArray();

    public DateTime CreatedDate { get; init; } = DateTime.Now;

    public DateTime? ExpirationDate { get; init; } = default!;

    public bool IsExpired => ExpirationDate != null && ExpirationDate.Value < DateTime.Now;
}