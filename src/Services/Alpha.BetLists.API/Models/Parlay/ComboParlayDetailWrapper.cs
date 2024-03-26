using Fanex.BetList.Core.App_GlobalResources;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Utils;

namespace Alpha.BetLists.API.Models.Parlay
{
    public class ComboParlayDetailWrapper
    {
        private readonly IDictionary<string, string> statusToResources = new Dictionary<string, string>
        {
            { BetStatus.Running, "Running" },
            { BetStatus.Reject, "Reject" },
            { BetStatus.Won, "Won" },
            { BetStatus.Lose, "Lost" },
            { BetStatus.Draw, "Draw" },
            { BetStatus.Waiting, "Waiting" },
            { BetStatus.Void, "Void" },
            { BetStatus.Refund, "Refund" },
            { BetStatus.Pending, CoreBetList.pending }
        };

        public ComboParlayDetailWrapper(
            ComboParlayDetail comboParlayDetail,
            IEnumerable<MatchDetail> matchDetails,
            ITicketHelper ticketHelper)
        {
            ComboParlayDetail = comboParlayDetail;
            MatchDetails = matchDetails;
            Odds = GetOdds();
            MasterOdds = ConvertByBetType.CommValue(comboParlayDetail.MasterOdds);
            BoostRate = ConvertByBetType.FormatBoostRate(comboParlayDetail.ExtraValue);

            string statusResource;
            Status = statusToResources.TryGetValue(ComboParlayDetail.Status?.ToLowerInvariant() ?? string.Empty, out statusResource) ? statusResource : string.Empty;
            Events = GetEvents(ticketHelper);
            WinLostDate = Formatter.ShortDateFormat(comboParlayDetail.WinLostDate);
            WinLost = ConvertByBetType.Winloss(comboParlayDetail.WinLost);
            Comm = ConvertByBetType.Comm(comboParlayDetail.PlayerComm);
            Stake = ConvertByBetType.Stake(comboParlayDetail.Stake);
        }

        public ComboParlayDetail ComboParlayDetail { get; }

        public IEnumerable<MatchDetail> MatchDetails { get; }

        public string WinLostDate { get; }

        public string WinLost { get; }

        public string Comm { get; }

        public string Status { get; }

        public string Stake { get; }

        public string Odds { get; }
        
        public string MasterOdds { get; }

        public string BoostRate { get; }

        public List<string> Events { get; }

        private List<string> GetEvents(ITicketHelper ticketHelper)
        {
            var events = new List<string>();

            var currentSerial = ComboParlayDetail.Serial;
            var matches = MatchDetails.Where(m => m.Serial == currentSerial);

            foreach (var match in matches)
            {
                var neutral = match.IsNeutral ? $"(N) " : string.Empty;
                events.Add($"{ticketHelper.GetTeamNameById(match.HomeId)} {neutral}- vs - {ticketHelper.GetTeamNameById(match.AwayId)}");
            }

            return events;
        }

        private string GetOdds()
            => ConvertByBetType.CommValue(ComboParlayDetail.Odds);

    }
}