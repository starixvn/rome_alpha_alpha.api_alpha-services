using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Models.Parlay
{
    public class ComboParlayViewModel
    {
        public ComboParlayViewModel(
            ParlayMeta meta,
            BetListData betListData,
            ITicketHelper ticketHelper)
        {
            ticketHelper.IsShowOddsWithChoice = true;
            HideOfferNameUnitStakes = string.IsNullOrEmpty(meta.BetCheck);
            ComboParlayOfferNames = string.IsNullOrEmpty(meta.BetCheck) ?
                new List<string>() :
                ticketHelper.GetCombinationMPOfferNameList(meta.BetTeam, meta.BetCheck, meta.Handicap1, meta.Handicap2);
            BetListData = betListData;
            ComboParlaySingleTickets = betListData.Tickets
                .Where(t => t.Stake > 0)
                .OrderBy(t => t.MatchId)
                .ToList();

            UnitStakeByOfferNames = GetUnitStakeByOfferName(meta.Handicap1);
        }

        public IDictionary<string, string> UnitStakeByOfferNames { get; }

        private IList<string> ComboParlayOfferNames { get; }

        private List<TicketDto> ComboParlaySingleTickets { get; }
        public BetListData BetListData { get; }

        private bool HideOfferNameUnitStakes { get; }

        public bool HasValidData
            => ComboParlaySingleTickets.Any() &&
            (HideOfferNameUnitStakes || ComboParlayOfferNames.Count <= ComboParlaySingleTickets.Count);

        private IDictionary<string, string> GetUnitStakeByOfferName(decimal handicap1)
        {
            var offerNameUnitStakes = new Dictionary<string, string>();
            if (HideOfferNameUnitStakes)
            {
                return offerNameUnitStakes;
            }

            var isSystemParlay = handicap1 == 0;

            if (HasValidData)
            {
                var offerNamesAmount = ComboParlayOfferNames.Count;
                for (var i = 0; i < offerNamesAmount; i++)
                {
                    var comboTicket = isSystemParlay ? ComboParlaySingleTickets[0] : ComboParlaySingleTickets[i];

                    offerNameUnitStakes.Add(ComboParlayOfferNames[i], comboTicket.Currency + " " + comboTicket.Stake.ToString("N0"));
                }
            }

            return offerNameUnitStakes;
        }
    }
    public class ParlayMeta
    {
        public decimal Handicap1 { get; set; }

        public decimal Handicap2 { get; set; }

        public string BetCheck { get; set; }

        public string BetTeam { get; set; }
    }
}