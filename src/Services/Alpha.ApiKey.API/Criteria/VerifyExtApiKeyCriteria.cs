using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class VerifyExtApiKeyCriteria : NonQueryCommand
{
    public VerifyExtApiKeyCriteria(string groupNames, string username, string password)
    {
        GroupNames = groupNames;
        Username = username;
        UserPassword = password;
    }

    public string GroupNames { get; }

    public string Username { get; }

    public string UserPassword { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }
    
    public override string GetSettingKey()
    {
        return "EAS_API_CheckSecretKeyUser";
    }

    public override bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(GroupNames)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(UserPassword);
    }
}