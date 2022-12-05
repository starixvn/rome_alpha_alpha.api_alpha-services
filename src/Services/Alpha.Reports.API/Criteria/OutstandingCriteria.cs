using Alpha.Utilities.Models;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public class OutstandingCriteria : CriteriaBase
{
    public OutstandingCriteria(byte custLevel, int custId, string productIds)
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
        => "Age_Rpt_Outstanding_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);

    public override bool IsValid() => !UserLevelModel.IsMemberLevel(CustLevel);
}