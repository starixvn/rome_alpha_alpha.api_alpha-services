using Fanex.BetDetails.BetBuilder.Criteria;
using Fanex.Data;
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria
{
    public class GetBetBuilderDetailCriteria : CriteriaBase
    {
        public GetBetBuilderDetailCriteria(IGetBetBuilderDetailParameter commandData)
        {
            Criteria = commandData;
        }

        [SpParam(Ignored = true)]
        private IGetBetBuilderDetailParameter Criteria { get; set; }

        public long CustId => Criteria.CustomerId;

        public long TransId => Criteria.TransactionId;

        public override string GetSettingKey() => "Acc_BetList_BetBuilder_Detail";

        public override bool IsValid() => Criteria.IsValid();
    }
}