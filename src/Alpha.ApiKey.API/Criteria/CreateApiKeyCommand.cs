using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class CreateApiKeyCommand : NonQueryCommand
{
    public CreateApiKeyCommand(int custId, string apiName, string passPhrase, string scopeList, string apiKey, string apiSecret, DateTime? expirationDate)
    {
        CustId = custId;
        ApiName = apiName;
        PassPhrase = passPhrase;
        ScopeList = scopeList;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
        ExpiredDate = expirationDate;
    }

    public int CustId { get; }

    public string ApiName { get; }

    public string PassPhrase { get; }

    public string ScopeList { get; }

    public string ApiKey { get; }

    public string ApiSecret { get; }

    public DateTime? ExpiredDate { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Acc_ManageAPI_Ins";

    public override bool IsValid()
        => CustId > 0
        && !string.IsNullOrWhiteSpace(ApiName)
        && !string.IsNullOrWhiteSpace(PassPhrase)
        && !string.IsNullOrWhiteSpace(ScopeList)
        && !string.IsNullOrWhiteSpace(ApiKey)
        && !string.IsNullOrWhiteSpace(ApiSecret);
}