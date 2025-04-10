namespace Alpha.Reports.API.Criteria;

public class GetMemberWinLossDetailCriteria : ReportProductCriteria
{
    public GetMemberWinLossDetailCriteria(
        int custId,
        DateTime balanceUpDay,
        DateTime moveBetsDay,
        IEnumerable<int> productIds,
        string productDetails,
        DateTime fromDate,
        DateTime toDate) : base(custId, balanceUpDay, moveBetsDay, productIds)
    {
        ProductDetails = productDetails;
        FromDate = fromDate;
        ToDate = toDate;
    }

    public string ProductDetails { get; }

    public DateTime FromDate { get; }

    public DateTime ToDate { get; }

    public override string GetSettingKey() => "Age_Rpt_API_WinLossDetail_Member";

    public override bool IsValid() => true;
}