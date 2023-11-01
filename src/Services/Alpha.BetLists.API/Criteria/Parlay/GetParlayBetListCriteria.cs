using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria.Parlay
{
    public class GetParlayBetListCriteria : CriteriaBase
    {
        public GetParlayBetListCriteria(
            long transId,
            long custId)
        {
            TransId = transId;
            CustId = custId;
        }

        public long TransId { get; }

        public long CustId { get; }

        public override string GetSettingKey() => "Age_BetList_SelParlayDetail";

        public override bool IsValid() => TransId > 0 && CustId > 0;
    }
}