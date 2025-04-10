using Alpha.Utilities.Models;
using Fanex.Data;

namespace Alpha.Reports.API.Criteria;

public class GetOutstandingCriteria : GetMemberOutstandingCriteria
{
    public GetOutstandingCriteria(
        byte custLevel,
        int custId,
        string productIds,
        int level) : base(custId, productIds)
    {
        CustLevel = custLevel;
        LevelType = level;
    }

    [SpParam(Ignored = true)]
    public byte CustLevel { get; }

    public int LevelType { get; }

    public override string GetSettingKey()
        => "Age_Rpt_Outstanding_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);

    public override bool IsValid() => !UserLevelModel.IsMemberLevel(CustLevel);
}
