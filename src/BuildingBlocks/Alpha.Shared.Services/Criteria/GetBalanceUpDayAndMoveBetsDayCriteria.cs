using Fanex.Data.Repository;

namespace Alpha.Shared.Services.Criteria;

/// <summary>
/// Shared criteria for retrieving balance up day and move bets day information
/// Used across Alpha.Reports.API and Alpha.BetLists.API
/// </summary>
public class GetBalanceUpDayAndMoveBetsDayCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Age_SelBalanceUpDay";

    public override bool IsValid() => true;
}
