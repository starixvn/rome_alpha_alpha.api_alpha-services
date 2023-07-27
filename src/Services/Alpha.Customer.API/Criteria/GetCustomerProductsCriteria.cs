using Fanex.Data.Repository;

namespace Alpha.Customer.API.Criteria;

public class GetCustomerProductsCriteria : CriteriaBase
{
    public GetCustomerProductsCriteria(
        int custId,
        int roleId,
        int settingStatus)
    {
        CustId = custId;
        RoleId = roleId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public int RoleId { get; }

    public int SettingStatus { get; }

    public override string GetSettingKey() => "Acc_CustSetting_StatusSelect";

    public override bool IsValid() => CustId >= 0 && RoleId > 0;
}
