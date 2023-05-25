
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria;

public class GetBalanceUpDayAndMoveBetsDayCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Age_SelBalanceUpDay";

    public override bool IsValid() => true;
}