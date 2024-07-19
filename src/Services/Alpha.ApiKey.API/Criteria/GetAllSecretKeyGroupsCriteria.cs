using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class GetAllSecretKeyGroupsCriteria : CriteriaBase
{
    public GetAllSecretKeyGroupsCriteria()
    {
    }

    public override string GetSettingKey() => "EAS_API_SelGroupBySecretKey";

    public override bool IsValid() => true;
}