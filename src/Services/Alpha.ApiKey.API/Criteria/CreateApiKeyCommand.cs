using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class CreateApiKeyCommand : NonQueryCommand
{
    public CreateApiKeyCommand(int custId, string apiName, string passPhrase, string scopeIDList, string apiKey, string apiSecret, string ip, string language, DateTime? expirationDate)
    {
        CustId = custId;
        ApiName = apiName;
        PassPhrase = passPhrase;
        ScopeIDList = scopeIDList;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
        IP = ip;
        Language = language;
        ExpiredDate = expirationDate;
    }

    public int CustId { get; }

    public string ApiName { get; }

    public string PassPhrase { get; }

    public string ScopeIDList { get; }

    public string ApiKey { get; }

    public string ApiSecret { get; }

    public string IP { get; }

    public string Language { get; }

    public DateTime? ExpiredDate { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_ManageAPI_Ins";

    public override bool IsValid()
    {
        if (CustId <= 0)
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(ApiName))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(PassPhrase))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(ScopeIDList))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(ApiKey))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(ApiSecret))
        {
            return false;
        }
        return true;
    }
}