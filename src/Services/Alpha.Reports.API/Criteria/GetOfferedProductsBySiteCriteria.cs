using Fanex.Data.Repository;
using Fanex.Data;

namespace Alpha.Reports.API.Criteria;

public class GetOfferedProductsBySiteCriteria : CriteriaBase
{
    public GetOfferedProductsBySiteCriteria(
        int custId,
        string site,
        int currencyId,
        byte settingStatus)
    {
        CustId = custId;
        Site = site;
        CurrencyId = currencyId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public string Site { get; }

    [SpParam(SqlParamName = "Currency")]
    public int CurrencyId { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey() => "Acc_CustSetting_CheckMacauEnable";

    public override bool IsValid() => !string.IsNullOrEmpty(Site) && CurrencyId > 0;
}