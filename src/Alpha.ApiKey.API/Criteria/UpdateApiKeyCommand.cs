using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class UpdateApiKeyCommand : NonQueryCommand
{
    public UpdateApiKeyCommand(int custId, string apiKey, string apiName, string scopeList, DateTime? expiredDate)
    {
        CustId = custId;
        ApiKey = apiKey;
        ApiName = apiName;
        ScopeList = scopeList;
        ExpiredDate = expiredDate;
    }

    public int CustId { get; }

    public string ApiName { get; }

    public string ScopeList { get; }

    public string ApiKey { get; }

    public DateTime? ExpiredDate { get; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Acc_ManageAPI_Upd";

    public override bool IsValid()
        => CustId > 0
        && !string.IsNullOrWhiteSpace(ApiName)
        && !string.IsNullOrWhiteSpace(ScopeList)
        && !string.IsNullOrWhiteSpace(ApiKey);
}