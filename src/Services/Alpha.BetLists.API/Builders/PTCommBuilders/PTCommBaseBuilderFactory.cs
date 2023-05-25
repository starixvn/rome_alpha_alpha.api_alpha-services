using Alpha.BetLists.API.Helpers;
using Alpha.BetLists.API.Models;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Builders.PTCommBuilders;

public static class PTCommBaseBuilderFactory
{
    public static PTCommBuilderBase CreateInstance(ITicket ticket, List<ITicketData> ticketData, UserLevel userLevel)
    {
        if (ticket.BetTypeId == 33 || ticket.BetTypeId == 1233)
        {
            return new PTCommBuilder33(ticket, ticketData, userLevel).InitPtCommData();
        }

        if (ticket.BetTypeId == 500)
        {
            return new PTCommBuilder500(ticket, userLevel).InitPtCommData();
        }

        if (ticket.BetTypeId == 501)
        {
            return new PTCommBuilder501(ticket, userLevel).InitPtCommData();
        }

        if (ticket.BetTypeId == 71 || ticket.BetTypeId == 73)
        {
            return new PTCommBuilder71(ticket, userLevel).InitPtCommData();
        }

        if (BetTypeHelper.IsJackpotContribution(ticket.BetTypeId))
        {
            return new PTCommBuilder2198(ticket, userLevel).InitPtCommData();
        }

        return new PTCommBuilderBase(ticket, userLevel).InitPtCommData();
    }
}