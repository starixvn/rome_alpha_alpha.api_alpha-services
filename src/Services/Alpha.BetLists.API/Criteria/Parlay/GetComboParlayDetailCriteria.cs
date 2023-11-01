using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria.Parlay
{
    public class GetComboParlayDetailCriteria : CriteriaBase
    {
        public GetComboParlayDetailCriteria(
            long refNo,
            int custId,
            DateTime balanceUpDate,
            DateTime moveBetDate)
        {
            RefNo = refNo;
            CustId = custId;
            BalanceUpDay = balanceUpDate;
            MoveBetDay = moveBetDate;
        }

        public long RefNo { get; }

        public int CustId { get; }

        public DateTime BalanceUpDay { get; }

        public DateTime MoveBetDay { get; }

        public decimal Outstanding { get; set; }
        
        public bool IsParlayBoost { get; set; }

        public override string GetSettingKey() => "Acc_BetlistCombinationParlay_Details";

        public override bool IsValid() => RefNo > 0;
    }
}