
using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Params;

public class GetOutstandingReportParam
{
    public GetOutstandingReportParam(
        byte viewerLevel,
        byte custLevel,
        int custId,
        string productIds,
        bool autoFollowBetIsDummyAgent,
        string siteName)
    {
        ViewerLevel = viewerLevel;
        CustLevel = custLevel;
        CustId = custId;
        ProductIds = productIds;
        AutoFollowBetIsDummyAgent = autoFollowBetIsDummyAgent;
        SiteName = siteName;
    }

    public byte ViewerLevel { get; }
    public int CustId { get; }
    public byte CustLevel { get; }
    public string ProductIds { get; }
    public bool AutoFollowBetIsDummyAgent { get; }
    public string SiteName { get; }

    public bool IsValid()
        => CustId > 0 && !UserLevelModel.IsUpline(CustLevel, ViewerLevel);
}