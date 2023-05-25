using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria;

public class GetIPRangesInfoByIPV6List : CriteriaBase
{
    public GetIPRangesInfoByIPV6List(string ipList)
    {
        IPList = ipList;
    }

    public string IPList { get; }

    public override string GetSettingKey() => "Age_IPRangesByCountrySelByIPV6List";

    public override bool IsValid() => true;
}