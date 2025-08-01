using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Utils;

namespace Alpha.BetLists.API.Models.Parlay
{
    public class ComboParlayDetailViewModel
    {
        private readonly Dictionary<string, int> statusOrder = new Dictionary<string, int>
        {
            { string.Empty, 0 },
            { BetStatus.Won, 1 },
            { BetStatus.Lose, 2 },
            { BetStatus.Draw, 3 },
            { BetStatus.Void, 4 },
            { BetStatus.Reject, 5 },
            { BetStatus.Refund, 6 }
        };

        // Place all constructors together (adjacent)
        public ComboParlayDetailViewModel(
            ComboParlayDetailData comboParlayDetailData,
            ITicketHelper ticketHelper)
        {
            ComboParlayDetailWrappers = new List<ComboParlayDetailWrapper>(comboParlayDetailData.ComboParlayDetails
                    .OrderBy(i => statusOrder[i.Status?.ToLowerInvariant() ?? string.Empty])
                    .Select(d => new ComboParlayDetailWrapper(d, comboParlayDetailData.MatchDetails, ticketHelper)));
            ComboParlayDetailWrapperGroups = ComboParlayDetailWrappers.GroupBy(w => w.WinLostDate).OrderBy(i => i.Key);
            IsParlayBoost = comboParlayDetailData.IsParlayBoost;

            OutStanding = ConvertByBetType.Stake(comboParlayDetailData.Outstanding);
            TotalWinLost = ConvertByBetType.Winloss(comboParlayDetailData.ComboParlayDetails.Sum(d => d.WinLost + d.PlayerComm));
            IsRunningTicket = !comboParlayDetailData.ComboParlayDetails.Any();
        }

        public ComboParlayDetailViewModel(string totalWinLost, string outStanding, string title, bool isDataTooHuge, bool isRunningTicket)
        {
            this.TotalWinLost = totalWinLost;
            this.OutStanding = outStanding;
            this.Title = title;
            this.IsDataTooHuge = isDataTooHuge;
            this.IsRunningTicket = isRunningTicket;
        }

        public IEnumerable<ComboParlayDetailWrapper> ComboParlayDetailWrappers { get; }

        public IEnumerable<IGrouping<string, ComboParlayDetailWrapper>> ComboParlayDetailWrapperGroups { get; }

        public string TotalWinLost { get; }

        public string OutStanding { get; }

        public bool IsParlayBoost { get; }

        public string Title { get; }

        public bool IsDataTooHuge { get; }

        public bool IsRunningTicket { get; }

        public string SubTotalWinLost(IEnumerable<ComboParlayDetailWrapper> group)
            => ConvertByBetType.Winloss(group.Sum(item => item.ComboParlayDetail.WinLost + item.ComboParlayDetail.PlayerComm));
    }
}