using Fanex.Data.Repository;

namespace Alpha.Customer.API.Criteria;

public class GetAllProductCriteria : CriteriaBase
{
    public GetAllProductCriteria(string siteName, int currency)
    {
        SiteName = siteName;
        Currency = currency;
    }

    public string SiteName { get; set; }

    public int Currency { get; set; }

    public override string GetSettingKey() => "EAS_API_GetProductList";

    public override bool IsValid() => true;
}
