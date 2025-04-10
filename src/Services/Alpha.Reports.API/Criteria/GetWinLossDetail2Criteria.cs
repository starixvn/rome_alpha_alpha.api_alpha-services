using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Criteria;

public class GetWinLossDetail2Criteria : GetWinLossDetailCriteria
{
    public GetWinLossDetail2Criteria(
        int custId,
        DateTime balanceUpDay,
        DateTime moveBetsDay,
        IEnumerable<int> productIds,
        byte custLevel,
        string productDetails,
        DateTime fromDate,
        DateTime toDate,
        string username,
        bool isReward,
        int pageIndex,
        int pageSize) : base(custId, balanceUpDay, moveBetsDay, productIds, custLevel, productDetails, fromDate, toDate, 0)
    {
        Username = username;
        IsReward = isReward;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public string Username { get; }

    public bool IsReward { get; }

    public int PageIndex { get; }

    public int PageSize { get; }

    public int TotalRecords { get; set;}

    public override string GetSettingKey()
        => "Age_Rpt_WinLossDetail_" + UserLevelModel.GetDirectDownlineLevelName(CustLevel);

    public override bool IsValid() => base.IsValid();
}