using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class CreateApiKeyCommand : NonQueryCommand
{
    public CreateApiKeyCommand(int custId, string apiName, string passPhrase, string scopeIDList, string apiKey, string apiSecret, string ip, DateTime? expirationDate)
    {
        CustId = custId;
        ApiName = apiName;
        PassPhrase = passPhrase;
        ScopeIDList = scopeIDList;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
        IP = ip;
        ExpiredDate = expirationDate;
    }

    public int CustId { get; }

    public string ApiName { get; }

    public string PassPhrase { get; }

    public string ScopeIDList { get; }

    public string ApiKey { get; }

    public string ApiSecret { get; }

    public string IP { get; }

    public DateTime? ExpiredDate { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_ManageAPI_Ins";

    public override bool IsValid()
        => CustId > 0
        && !string.IsNullOrWhiteSpace(ApiName)
        && !string.IsNullOrWhiteSpace(PassPhrase)
        && !string.IsNullOrWhiteSpace(ScopeIDList)
        && !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(ApiSecret);
}