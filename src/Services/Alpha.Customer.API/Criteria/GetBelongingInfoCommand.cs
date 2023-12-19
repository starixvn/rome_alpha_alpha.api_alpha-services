using Fanex.Data.Repository;

namespace Alpha.Customer.API.Criteria;

public class GetBelongingInfoCommand : NonQueryCommand
{
    public GetBelongingInfoCommand(int custId, int sessionCustId, int subAccountId, long transId)
    {
        CustId = custId;
        SubAccountId = subAccountId;
        SessionCustId = sessionCustId;
        TransId = transId;
    }

    public int SessionCustId { get; }

    public int CustId { get; }

    public int SubAccountId { get; }

    public long? TransId { get; }

    public byte RoleId { get; set; }

    public string Username { get; set; }

    public bool IsActive { get; set; }

    public override string GetSettingKey() => "Age_CustomerSelCheckBelonging";

    public override bool IsValid() => SessionCustId > 0 && TransId > 0;
}
