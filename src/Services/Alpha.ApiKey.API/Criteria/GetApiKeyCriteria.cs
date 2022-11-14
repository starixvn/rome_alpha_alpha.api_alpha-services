using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Criteria;

public class GetApiKeyCriteria : CriteriaBase
{
    public GetApiKeyCriteria(string apiKey)
    {
        ApiKey = apiKey;
    }

    public string ApiKey { get; }

    public override string GetSettingKey() => "Age_ManageAPI_SelByKey";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(ApiKey);
}