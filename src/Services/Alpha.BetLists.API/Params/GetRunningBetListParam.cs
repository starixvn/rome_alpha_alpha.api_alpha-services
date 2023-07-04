using Alpha.BetLists.API.Extensions;

namespace Alpha.BetLists.API.Params;

public class GetRunningBetListParam
{
    public GetRunningBetListParam(
        int custId,
        string language,
        string userName,
        string siteName,
        IEnumerable<int> productIds)
    {
        CustId = custId;
        Language = language;
        UserName = userName;
        ProductIds = productIds;
        SiteName = siteName;
    }

    public int CustId { get; }

    public string Language { get; }

    public string UserName { get; }

    public string SiteName { get; }

    public IEnumerable<int> ProductIds { get; }

    public bool IsValid() => CustId > 0;
}