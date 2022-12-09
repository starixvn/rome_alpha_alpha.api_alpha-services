namespace Alpha.Reports.API.Criteria;

public class PagingReportProductCriteria : ReportProductCriteria
{
    public PagingReportProductCriteria(
        int custId,
        int pageIndex,
        int pageSize,
        DateTime balanceUpDay,
        DateTime moveBetsDay,
        IEnumerable<int> productIds) : base(custId, balanceUpDay, moveBetsDay, productIds)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public int PageIndex { get; }

    public int PageSize { get; }

    public int TotalRecords { get; set; }

    public override bool IsValid() => base.IsValid() && PageIndex >= 0;
}