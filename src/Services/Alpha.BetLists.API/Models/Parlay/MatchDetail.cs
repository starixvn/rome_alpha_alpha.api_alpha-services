namespace Alpha.BetLists.API.Models.Parlay
{
    public class MatchDetail
    {
        public MatchDetail()
        {
        }

        public MatchDetail(long serial, int homeId, int awayId, bool isNeutral)
        {
            Serial = serial;
            HomeId = homeId;
            AwayId = awayId;
            IsNeutral = isNeutral;
        }

        public long Serial { get; }

        public int HomeId { get; }

        public int AwayId { get; }

        public bool IsNeutral { get; }
    }
}