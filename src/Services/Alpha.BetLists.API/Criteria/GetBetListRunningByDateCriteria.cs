using System;
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria;

public class GetBetListRunningByDateCriteria : CriteriaBase
{
    public GetBetListRunningByDateCriteria(int custId, string productIds)
    {
        CustId = custId;
        ProductIds = productIds;
        GetAll = true;
    }

    public int CustId { get; }

    public DateTimeOffset FromDate { get; } = DateTimeOffset.Now;

    public DateTimeOffset ToDate { get; } = DateTimeOffset.Now;

    public bool GetAll { get; }

    public string ProductIds { get; }

    public override string GetSettingKey() => "Age_BetList_SelRunning_Mem";

    public override bool IsValid() =>
        CustId > 0
        && !string.IsNullOrWhiteSpace(ProductIds)
        && ToDate >= FromDate
        && FromDate > DateTimeOffset.MinValue;
}