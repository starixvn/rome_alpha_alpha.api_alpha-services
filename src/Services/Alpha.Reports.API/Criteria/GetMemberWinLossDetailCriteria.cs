using Alpha.Utilities.Models;
using Fanex.Data;

namespace Alpha.Reports.API.Criteria;

public class GetMemberWinLossDetailCriteria : ReportProductCriteria
{
    public GetMemberWinLossDetailCriteria(
        int custId,
        DateTimeOffset balanceUpDay,
        DateTimeOffset moveBetsDay,
        IEnumerable<int> productIds,
        byte custLevel,
        string productDetails,
        DateTimeOffset fromDate,
        DateTimeOffset toDate)
        : base(custId, balanceUpDay, moveBetsDay, productIds)
    {
        CustLevel = custLevel;
        ProductDetails = productDetails;
        FromDate = fromDate;
        ToDate = toDate;
    }

    [SpParam(Ignored = true)]
    public byte CustLevel { get; }

    public string ProductDetails { get; }

    public DateTimeOffset FromDate { get; }

    public DateTimeOffset ToDate { get; }

    public override string GetSettingKey()
        => "Age_Rpt_API_WinLossDetail_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);

    public override bool IsValid() => true;
}