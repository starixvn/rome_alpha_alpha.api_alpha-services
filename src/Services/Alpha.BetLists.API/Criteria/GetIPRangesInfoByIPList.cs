using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria;

public class GetIPRangesInfoByIPList : CriteriaBase
{
    public GetIPRangesInfoByIPList(string ipList)
    {
        IPList = ipList;
    }

    public string IPList { get; }

    public override string GetSettingKey() => "Age_IPRangesByCountrySelByIPList";

    public override bool IsValid() => true;
}