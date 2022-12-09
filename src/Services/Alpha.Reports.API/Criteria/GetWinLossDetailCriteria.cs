using Alpha.Utilities.Models;
using Fanex.Data;

namespace Alpha.Reports.API.Criteria;

public class GetWinLossDetailCriteria : PagingReportProductCriteria
{
    public GetWinLossDetailCriteria(
        int custId,
        int pageIndex,
        int pageSize,
        DateTime balanceUpDay,
        DateTime moveBetsDay,
        IEnumerable<int> productIds,
        byte custLevel,
        string productDetails)
        : base(custId, pageIndex, pageSize, balanceUpDay, moveBetsDay, productIds)
    {
        CustLevel = custLevel;
        ProductDetails = productDetails;
    }

    [SpParam(Ignored = true)]
    public byte CustLevel { get; }

    public string ProductDetails { get; }

    public override string GetSettingKey()
        => "Age_Rpt_API_WinLossDetail_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);

    public override bool IsValid() => true;
}