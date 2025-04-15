using Alpha.Utilities.Models;
using Fanex.Data;

namespace Alpha.Reports.API.Criteria;

public class GetMemberWinLossDetailCriteria : ReportProductCriteria
{
    public GetMemberWinLossDetailCriteria(
        int custId,
        DateTime balanceUpDay,
        DateTime moveBetsDay,
        IEnumerable<int> productIds,
        byte custLevel,
        string productDetails,
        DateTime fromDate,
        DateTime toDate)
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

    public DateTime FromDate { get; }

    public DateTime ToDate { get; }

    public override string GetSettingKey()
        => "Age_Rpt_API_WinLossDetail_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);

    public override bool IsValid() => true;
}