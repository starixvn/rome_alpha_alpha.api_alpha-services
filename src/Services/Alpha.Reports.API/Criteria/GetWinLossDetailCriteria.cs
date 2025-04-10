using Alpha.Utilities.Models;
using Fanex.Data;

namespace Alpha.Reports.API.Criteria;

public class GetWinLossDetailCriteria : GetMemberWinLossDetailCriteria
{
    public GetWinLossDetailCriteria(
        int custId,
        DateTime balanceUpDay,
        DateTime moveBetsDay,
        IEnumerable<int> productIds,
        byte custLevel,
        string productDetails,
        DateTime fromDate,
        DateTime toDate,
        int level) : base(custId, balanceUpDay, moveBetsDay, productIds, productDetails, fromDate, toDate)
    {
        CustLevel = custLevel;
        LevelType = level;
    }

    [SpParam(Ignored = true)]
    public byte CustLevel { get; }

    public int LevelType { get; }

    public override string GetSettingKey()
        => "Age_Rpt_API_WinLossDetail_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);
}