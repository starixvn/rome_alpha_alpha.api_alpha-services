using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class UpdateApiKeyCommand : NonQueryCommand
{
    public UpdateApiKeyCommand(int custId, string apiKey, string apiName, string scopeIDList, string ip, DateTime? expiredDate)
    {
        CustId = custId;
        ApiKey = apiKey;
        ApiName = apiName;
        ScopeIDList = scopeIDList;
        IP = ip;
        ExpiredDate = expiredDate;
    }

    public int CustId { get; }

    public string ApiName { get; }

    public string ScopeIDList { get; }

    public string ApiKey { get; }

    public string IP { get; }

    public DateTime? ExpiredDate { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_ManageAPI_Upd";

    public override bool IsValid()
        => CustId > 0
        && !string.IsNullOrWhiteSpace(ApiName)
        && !string.IsNullOrWhiteSpace(ScopeIDList)
        && !string.IsNullOrWhiteSpace(ApiKey);
}