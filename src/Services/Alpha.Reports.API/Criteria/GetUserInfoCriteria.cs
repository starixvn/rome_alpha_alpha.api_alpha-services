using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public class GetUserInfoCriteria : CriteriaBase
{
    public GetUserInfoCriteria(int custId, byte settingStatus)
    {
        CustId = custId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey() => "Age_CustomerSelUserInfo";

    public override bool IsValid() => CustId > 0;
}