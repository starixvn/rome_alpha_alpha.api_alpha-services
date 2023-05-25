using Alpha.BetLists.API.Models;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Utils;

namespace Alpha.BetLists.API.Builders.PTCommBuilders;

public class PTCommBuilder2198 : PTCommBuilderBase
{
    public PTCommBuilder2198(ITicket ticket, UserLevel userLevel)
             : base(ticket, userLevel)
    {
    }

    public override string PositionTaking => FormatPositionTaking(PtByUserLevel[UserLevel]);

    private static string FormatPositionTaking(object value)
    {
        var ptValue = Cast.AsDecimal(value);

        return ptValue < 0
            ? string.Format(ConvertByBetType.Minus, string.Format(Formatter.DECIMALFORMAT40, ptValue))
            : string.Format(Formatter.DECIMALFORMAT40, ptValue);
    }
}