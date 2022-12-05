
using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Params;

public class GetOutstandingReportParam
{
    public GetOutstandingReportParam(
        byte userLevel,
        byte custLevel,
        int custId,
        string productIds,
        bool autoFollowBetIsDummyAgent)
    {
        UserLevel = userLevel;
        CustLevel = custLevel;
        CustId = custId;
        ProductIds = productIds;
        AutoFollowBetIsDummyAgent = autoFollowBetIsDummyAgent;
    }

    public byte UserLevel { get; }
    public int CustId { get; }
    public byte CustLevel { get; }
    public string ProductIds { get; }
    public bool AutoFollowBetIsDummyAgent { get; }

    public bool IsValid()
        => CustId > 0 && !UserLevelModel.IsUpline(CustLevel, UserLevel);
}