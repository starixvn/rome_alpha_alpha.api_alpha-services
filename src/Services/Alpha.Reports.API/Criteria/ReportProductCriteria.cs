using Alpha.Utilities.Extensions;

namespace Alpha.Reports.API.Criteria;

public class ReportProductCriteria : ReportCriteriaBase
{
    public ReportProductCriteria(
        int custId,
        DateTimeOffset balanceUpDay,
        DateTimeOffset moveBetsDay,
        IEnumerable<int> productIds)
        : base(custId, balanceUpDay, moveBetsDay)
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