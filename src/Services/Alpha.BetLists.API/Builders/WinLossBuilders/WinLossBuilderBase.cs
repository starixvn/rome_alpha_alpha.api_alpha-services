
using Alpha.BetLists.API.Helpers;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Utils;

namespace Alpha.BetLists.API.Builders.WinLossBuilders;

public static class WinLossBuilder
{
    public static string BuildWinLoss(ITicket ticket)
    {
        var result = $"<div><span class='bl_underdog'>{GetWinLoss(ticket)}</span></div>";

        if (!BetTypeHelper.IsJackpotContribution(ticket.BetTypeId))
        {
            result += $"<div><span class='bl_underdog'>{GetComm(ticket)}</span></div>";
        }

        return result;
    }

    private static string GetWinLoss(ITicket ticket)
    {
        if (BetTypeHelper.IsJackpotContribution(ticket.BetTypeId))
        {
            return "-";
        }

        var status = ticket.Status.ToLowerInvariant();

        return status switch
        {
            BetStatus.Running or BetStatus.Waiting or BetStatus.Pending => "-",
            BetStatus.Void => ticket.BetTypeId == 500 ? "(NW) 0.00" : string.Empty,
            BetStatus.Reject => "0.00",
            _ => ticket.BetTypeId == 500 ? "(NW) " + ConvertByBetType.Winloss(ticket.ActualStake) : ConvertByBetType.Winloss(ticket.Winlost),
        };
    }

    private static string GetComm(ITicket ticket)
    {
        if (BetTypeHelper.IsJackpotContribution(ticket.BetTypeId))
        {
            return string.Empty;
        }

        var status = ticket.Status.ToLowerInvariant();

        return status switch
        {
            BetStatus.Running or BetStatus.Waiting or BetStatus.Pending => string.Empty,
            BetStatus.Void => "0.00",
            BetStatus.Reject => string.Empty,
            _ => ticket.BetTypeId == 500 ? ConvertByBetType.Winloss(ticket.PlayerComm) : ConvertByBetType.Comm(ticket.PlayerComm),
        };
    }
}