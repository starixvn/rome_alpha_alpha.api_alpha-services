using Fanex.Data.Repository;

namespace Alpha.Customer.API.Criteria;

public class GetCustomerProductsCriteria : CriteriaBase
{
    public GetCustomerProductsCriteria(int custId, int settingStatus)
    {
        CustId = custId;
        SettingStatus = settingStatus;
    }

    public int CustId { get; }

    public int SettingStatus { get; }

    public override string GetSettingKey() => "Age_ManageAPI_CustProductSel";

    public override bool IsValid() => CustId >= 0;
}
