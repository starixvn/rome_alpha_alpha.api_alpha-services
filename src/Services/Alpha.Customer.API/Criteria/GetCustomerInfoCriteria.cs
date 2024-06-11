using Fanex.Data.Repository;

namespace Alpha.Customer.API.Criteria;

public class GetCustomerInfoCriteria : CriteriaBase
{
    public GetCustomerInfoCriteria(int custId, string username)
    {
        CustId = custId;
        Username = username;
    }

    public int CustId { get; }

    public string Username { get; }

    public override string GetSettingKey() => "Age_ManageAPI_CustInfo";

    public override bool IsValid()
        => CustId > 0 || !string.IsNullOrWhiteSpace(Username);
}
