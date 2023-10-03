using Fanex.Data;
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Criteria
{
    public class GetCasinoDetailsCriteria : CriteriaBase
    {
        public GetCasinoDetailsCriteria(
            long refNo,
            int custId,
            byte transType,
            int productId,
            int pageIndex,
            int pageSize,
            DateTime moveBetsDate,
            DateTime balanceUpDate)
        {
            RefNo = refNo;
            CustId = custId;
            TransType = transType;
            ProductId = productId;
            PageIndex = pageIndex;
            PageSize = pageSize;
            MoveBetsDate = moveBetsDate;
            BalanceUpDate = balanceUpDate;
        }

        public long RefNo { get; }

        public int CustId { get; }

        public byte TransType { get; }

        [SpParam(Ignored = true)]
        public int ProductId { get; }

        public int PageIndex { get; }

        public int PageSize { get; }

        public int TotalRows { get; set; }

        [SpParam(SqlParamName = "movebetsday")]
        public DateTime MoveBetsDate { get; }

        [SpParam(SqlParamName = "BalanceUpDay")]
        public DateTime BalanceUpDate { get; }

        public override string GetSettingKey()
            => "Acc_BetList_Detail_" + ProductId;

        public override bool IsValid()
            => RefNo > 0
            && CustId > 0
            && ProductId > 0;
    }
}