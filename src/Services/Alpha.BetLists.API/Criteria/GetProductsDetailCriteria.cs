using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria;

public class GetProductsDetailCriteria : CriteriaBase
{
    public override string GetSettingKey() => "Age_Rpt_GetProductDetail";

    public override bool IsValid() => true;
}