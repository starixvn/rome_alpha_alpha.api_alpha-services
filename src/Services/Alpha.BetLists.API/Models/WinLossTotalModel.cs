using Fanex.BetList.Core.Utils;

namespace Alpha.BetLists.API.Models;

public class WinLossTotalModel
{
    public string WinLoss { get; set; }
    public string Comm { get; set; }
    public string Total { get; set; }

    public static WinLossTotalModel GetWinLossTotal(BetListData b)
    {
        var comm = b.Tickets.Sum(ticket => ticket.PlayerComm);
        var winLoss = b.Tickets.Sum(ticket => ticket.Winlost);

        var result = new WinLossTotalModel
        {
            Comm = ConvertByBetType.Comm(comm),
            WinLoss = ConvertByBetType.Winloss(winLoss),
            Total = ConvertByBetType.Winloss(comm + winLoss)
        };

        return result;
    }
}