using Fanex.Data.Repository;

namespace Alpha.Reports.API.Criteria;

public class GetProductsBySiteCurrencyCriteria : CriteriaBase
{
    public GetProductsBySiteCurrencyCriteria(string site, int currencyId, byte settingStatus)
    {
        Site = site;
        Currency = currencyId;
        SettingStatus = settingStatus;
    }

    public string Site { get; }

    public int Currency { get; }

    public byte SettingStatus { get; }

    public override string GetSettingKey() => "Age_GetOfferedProductBySiteCurrency";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(Site) && Currency > 0;
}