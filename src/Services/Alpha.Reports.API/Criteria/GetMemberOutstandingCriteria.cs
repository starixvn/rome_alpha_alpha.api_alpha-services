using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public class GetMemberOutstandingCriteria : CriteriaBase
{
    public GetMemberOutstandingCriteria(int custId, string productIds)
    {
        CustId = custId;
        ProductIds = productIds;
    }

    public int CustId { get; }

    public string ProductIds { get; }

    public override string GetSettingKey() => "Age_Rpt_Outstanding_Member";

    public override bool IsValid() => true;
}