using Alpha.BetLists.API.Models;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Builders.PTCommBuilders;

public class PTCommBuilder500 : PTCommBuilderBase
{
    public PTCommBuilder500(ITicket ticket, UserLevel userLevel) : base(ticket, userLevel)
    {
    }

    protected override void InitCommSettingByUserLevel()
    {
        CommSettingByUserLevel = new Dictionary<UserLevel, object>
        {
            { UserLevel.Super, Ticket.SuperDiscount - Ticket.MasterDiscount },
            { UserLevel.Master, Ticket.MasterDiscount - Ticket.AgentDiscount },
            { UserLevel.Agent, -Ticket.AgentDiscount }
        };
    }
}