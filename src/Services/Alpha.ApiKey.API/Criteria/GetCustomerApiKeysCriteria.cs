using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class GetCustomerApiKeysCriteria : CriteriaBase
{
    public GetCustomerApiKeysCriteria(int custId)
        => CustId = custId;

    public int CustId { get; }

    public override string GetSettingKey() => "Age_ManageAPI_SelByCust";

    public override bool IsValid() => CustId > 0;
}