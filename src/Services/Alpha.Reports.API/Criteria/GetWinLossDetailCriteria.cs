using Alpha.Utilities.Models;
using Fanex.Data;

namespace Alpha.Reports.API.Criteria;

public class GetWinLossDetailCriteria : ReportProductCriteria
{
    public GetWinLossDetailCriteria(
        int custId,
        DateTimeOffset balanceUpDay,
        DateTimeOffset moveBetsDay,
        IEnumerable<int> productIds,
        byte custLevel,
        string productDetails,
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        int level)
        : base(custId, balanceUpDay, moveBetsDay, productIds)
    {
        CustLevel = custLevel;
        ProductDetails = productDetails;
        FromDate = fromDate;
        ToDate = toDate;
        Level = level;
    }

    [SpParam(Ignored = true)]
    public byte CustLevel { get; }

    public string ProductDetails { get; }

    public DateTimeOffset FromDate { get; }

    public DateTimeOffset ToDate { get; }

    [SpParam(SqlParamName = "LevelType")]
    public int Level { get; }

    public override string GetSettingKey()
        => "Age_Rpt_API_WinLossDetail_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);

    public override bool IsValid() => true;
}