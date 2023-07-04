using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria;

public class GetIPRangesByCountryCriteria : CriteriaBase
{
    public GetIPRangesByCountryCriteria(bool isIPV6, string countryCodes)
    {
        IsIPV6 = isIPV6;
        CountryCodes = countryCodes;
    }

    public bool IsIPV6 { get; }
    public string CountryCodes { get; }

    public override string GetSettingKey() => "Age_IPRangesByCountrySel";

    public override bool IsValid() => true;
}