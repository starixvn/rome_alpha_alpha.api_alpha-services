using Alpha.Utilities.Models;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public class GetMemberOutstandingCriteria : CriteriaBase
{
    public GetMemberOutstandingCriteria(byte custLevel, int custId, string productIds)
    {
        CustLevel = custLevel;
        CustId = custId;
        ProductIds = productIds;
    }

    public int CustId { get; }

    [SpParam(Ignored = true)]
    public byte CustLevel { get; }

    public string ProductIds { get; }

    public override string GetSettingKey()
        => "Age_Rpt_Outstanding_Member";

    public override bool IsValid() => !UserLevelModel.IsMemberLevel(CustLevel);
}