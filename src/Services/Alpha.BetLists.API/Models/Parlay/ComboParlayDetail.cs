namespace Alpha.BetLists.API.Models.Parlay
{
    public class ComboParlayDetail
    {
        public ComboParlayDetail()
        {

        }

        public ComboParlayDetail(
            DateTime winlostDate,
            long transId,
            long serial,
            int statusId,
            string status,
            decimal modds,
            decimal stake,
            decimal winlost,
            decimal playerComm)
        {
            WinLostDate = winlostDate;
            TransId = transId;
            Serial = serial;
            StatusId = statusId;
            Status = status;
            Odds = modds;
            Stake = stake;
            WinLost = winlost;
            PlayerComm = playerComm;
        }

        public DateTime WinLostDate { get; }

        public long TransId { get; }

        public long Serial { get; }

        public int StatusId { get; }

        public string Status { get; }

        public decimal MasterOdds { get; }

        public decimal Odds { get; }

        public decimal Stake { get; }

        public decimal WinLost { get; }

        public decimal PlayerComm { get; }
        public decimal ExtraValue { get; }
    }
}