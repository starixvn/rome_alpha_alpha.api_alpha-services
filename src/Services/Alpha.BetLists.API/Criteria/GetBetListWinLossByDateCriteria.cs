
using Fanex.Data;
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria;

public class GetBetListWinLossByDateCriteria : CriteriaBase
{
    public GetBetListWinLossByDateCriteria(
        int custId,
        string productIds,
        DateTimeOffset fromDate,
        DateTimeOffset toDate,
        string productDetails,
        DateTimeOffset moveBetsDate,
        int maxTotalRow)
    {
        CustId = custId;
        FromDate = fromDate;
        ToDate = toDate;
        ProductIds = productIds;
        ProductDetails = productDetails;
        MoveBetsDate = moveBetsDate;
        MaxTotalRow = maxTotalRow;

        ShowProgressiveBonus = false;
        IsExportExcel = false;
    }

    public int CustId { get; }

    public DateTimeOffset FromDate { get; }

    public DateTimeOffset ToDate { get; }

    public string ProductIds { get; }

    public string ProductDetails { get; }

    [SpParam(SqlParamName = "movebetsday")]
    public DateTimeOffset MoveBetsDate { get; }

    public bool ShowProgressiveBonus { get; }

    public bool IsExportExcel { get; }

    public int MaxTotalRow { get; set; }

    public int ErrCode { get; set; }

    public string ErrMsg { get; set; }

    public override string GetSettingKey() => "Age_BetList_SelWinLossByDate_Mem";

    public override bool IsValid() =>
        CustId > 0
        && !string.IsNullOrWhiteSpace(ProductIds)
        && ToDate >= FromDate
        && FromDate > DateTimeOffset.MinValue;
}