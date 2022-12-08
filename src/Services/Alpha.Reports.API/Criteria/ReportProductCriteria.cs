
using Alpha.Reports.API.Extensions;

namespace Alpha.Reports.API.Criteria;

public class ReportProductCriteria : ReportCriteriaBase
{
    public ReportProductCriteria(
        int custId,
        DateTime fromDate,
        DateTime toDate,
        DateTime balanceUpDay,
        DateTime moveBetsDay,
        IEnumerable<int> productIds)
        : base(custId, fromDate, toDate, balanceUpDay, moveBetsDay)
    {
        ProductIds = productIds == null ? string.Empty : productIds.JoinWithComma();
    }

    public string ProductIds { get; }

    public override string GetSettingKey()
    {
        throw new NotImplementedException();
    }

    public override bool IsValid() => base.IsValid() && !string.IsNullOrWhiteSpace(ProductIds);
}