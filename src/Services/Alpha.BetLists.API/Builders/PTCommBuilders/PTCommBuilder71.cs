using Alpha.BetLists.API.Models;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Builders.PTCommBuilders;

public class PTCommBuilder71 : PTCommBuilderBase
{
    public PTCommBuilder71(ITicket ticket, UserLevel userLevel) : base(ticket, userLevel)
    {
    }

    public override bool VisibilePositionTakingPercent => false;

    public override bool VisibileCommisionPercent => false;
}