using Alpha.BetLists.API.Extensions;
using Alpha.BetLists.API.Models;

namespace Alpha.BetLists.API.Params;

public class GetWinLossBetListParam
{
    public GetWinLossBetListParam(
        int custId,
        int roleId,
        string fromDate,
        string toDate,
        string userName,
        string siteName,
        IEnumerable<int> productIds,
        int viewerId,
        BetListsAppSettingsModel settings)
    {
        CustId = custId;
        RoleId = roleId;

        DateTime dt;
        FromDate = DateTime.TryParse(fromDate, out dt) ? dt : DateTime.MinValue;
        ToDate = DateTime.TryParse(toDate, out dt) ? dt : DateTime.MinValue;

        UserName = userName;
        ProductIds = productIds;
        SiteName = siteName;

        ViewerId = viewerId;
        Settings = settings;
    }

    public int ViewerId { get; }

    public int RoleId { get; }

    public int CustId { get; }

    public DateTime FromDate { get; }

    public DateTime ToDate { get; }

    public string UserName { get; }

    public string SiteName { get; }

    public IEnumerable<int> ProductIds { get; }

    public int MaxTotalRow { get; set; } = 10000;

    public BetListsAppSettingsModel Settings { get; }

    public bool IsValid() => CustId > 0;
}