using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Builders.StatusBuilders;

public static class StatusBuilderBase
{
    public static string BuildStatus(
        this IBetListHTMLBuilder builder,
        ITicket ticket,
        ITicketHelper ticketHelper,
        List<ITicketData> ticketData,
        string country)
    {
        var result = builder.BuildStatus(ticket, ticketHelper, ticketData, true);
        result += $"<div class=\"country\">{country}</div>";
        return result;
    }
}