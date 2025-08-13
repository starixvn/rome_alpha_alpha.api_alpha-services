namespace Alpha.BetLists.API.Criteria;
using Fanex.Data.Repository;
public class GetBetBuilderDetailCriteria(long custId, long transId) : CriteriaBase
{
    public long CustId { get; } = custId;

    public long TransId { get; } = transId;

    public override string GetSettingKey() => "Acc_BetList_BetBuilder_Detail";

    public override bool IsValid() => CustId > 0 && TransId > 0;
}