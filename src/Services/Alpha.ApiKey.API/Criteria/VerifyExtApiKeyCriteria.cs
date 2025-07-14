using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class VerifyExtApiKeyCriteria : NonQueryCommand
{
    public VerifyExtApiKeyCriteria(string key, string username, string password)
    {
        APISecretKey = key;
        Username = username;
        UserPassword = password;
    }

    public string APISecretKey { get; }

    public string Username { get; }

    public string UserPassword { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public bool IsAuthenticated { get; set; }

    public int CustId { get; set; }

    public int SubAccId { get; set; }

    public string Site { get; set; }

    public int Currency { get; set; }

    public override string GetSettingKey() => "EAS_API_CheckSecretKeyUser";

    public override bool IsValid()
        => !string.IsNullOrWhiteSpace(APISecretKey)
        && !string.IsNullOrWhiteSpace(Username)
        && !string.IsNullOrWhiteSpace(UserPassword);
}