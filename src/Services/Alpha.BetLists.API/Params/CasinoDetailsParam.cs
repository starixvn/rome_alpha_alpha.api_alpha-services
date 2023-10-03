namespace Alpha.BetLists.API.Params
{
    public class CasinoDetailsParam
    {
        public CasinoDetailsParam(long refNo, int custId, byte transType, int productId, int pageIndex, int pageSize)
        {
            RefNo = refNo;
            CustId = custId;
            TransType = transType;
            ProductId = productId;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }
        public long RefNo { get; }

        public int CustId { get; }

        public byte TransType { get; }

        public int ProductId { get; }

        public int PageIndex { get; }

        public int PageSize { get; }
    }
}