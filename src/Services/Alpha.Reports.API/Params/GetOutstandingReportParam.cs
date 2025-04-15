
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
        string siteName,
        int level)
    {
        ViewerLevel = viewerLevel;
        CustLevel = custLevel;
        CustId = custId;
        ProductIds = productIds;
        AutoFollowBetIsDummyAgent = autoFollowBetIsDummyAgent;
        SiteName = siteName;
        Level = level;
    }

    public byte ViewerLevel { get; }
    public int CustId { get; }
    public byte CustLevel { get; }
    public string ProductIds { get; }
    public bool AutoFollowBetIsDummyAgent { get; }
    public string SiteName { get; }
    public int Level { get; }

    public bool IsValid()
        => CustId > 0 
        && !UserLevelModel.IsUpline(CustLevel, ViewerLevel)
        && 0 <= Level 
        && Level <= 2;
}