using Alpha.BetLists.API.Models;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.Utils;

namespace Alpha.BetLists.API.Builders.PTCommBuilders;

public class PTCommBuilderBase
{
    public PTCommBuilderBase(ITicket ticket, UserLevel userLevel)
    {
        Ticket = ticket ?? new Ticket();
        UserLevel = userLevel;
    }

    public virtual bool VisiblePositionTaking => !BetStatusIsRunningOrWaiting();

    public virtual bool VisibleCommision => !BetStatusIsRunningOrWaiting();

    public virtual bool VisibilePositionTakingPercent => true;

    public virtual bool VisibileCommisionPercent => true;

    public virtual string PositionTaking => ConvertByBetType.PtValue(PtByUserLevel[UserLevel]);

    public virtual string PositionTakingPercent => ConvertByBetType.Pt(PtSettingByUserLevel[UserLevel]) + "%";

    public virtual string Commission => ConvertByBetType.Comm(CommByUserLevel[UserLevel]);

    public virtual string CommissionPercent
        => GetCommisionPercentText(ConvertByBetType.Comm_Percentage(CommSettingByUserLevel[UserLevel]));

    protected ITicket Ticket { get; }

    protected UserLevel UserLevel { get; }

    protected Dictionary<UserLevel, object> PtByUserLevel { get; set; }

    protected Dictionary<UserLevel, object> PtSettingByUserLevel { get; set; }

    protected Dictionary<UserLevel, object> CommByUserLevel { get; set; }

    protected Dictionary<UserLevel, object> CommSettingByUserLevel { get; set; }

    public PTCommBuilderBase InitPtCommData()
    {
        InitPtByUserLevel();
        InitPtSettingByUserLevel();
        InitCommByUserLevel();
        InitCommSettingByUserLevel();

        return this;
    }

    protected virtual void InitPtByUserLevel()
    {
        PtByUserLevel = new Dictionary<UserLevel, object>
            {
                { UserLevel.Super, Ticket.SuperWinlost },
                { UserLevel.Master, Ticket.MasterWinlost },
                { UserLevel.Agent, Ticket.AgentWinlost }
            };
    }

    protected virtual void InitPtSettingByUserLevel()
    {
        PtSettingByUserLevel = new Dictionary<UserLevel, object>
            {
                { UserLevel.Super, Ticket.SuperPositionTaking },
                { UserLevel.Master, Ticket.MasterPositionTaking },
                { UserLevel.Agent, Ticket.AgentPositionTaking }
            };
    }

    protected virtual void InitCommByUserLevel()
    {
        CommByUserLevel = new Dictionary<UserLevel, object>
            {
                { UserLevel.Super, Ticket.SuperComm },
                { UserLevel.Master, Ticket.AgentComm },
                { UserLevel.Agent, Ticket.Comm }
            };
    }

    protected virtual void InitCommSettingByUserLevel()
    {
        CommSettingByUserLevel = new Dictionary<UserLevel, object>
            {
                { UserLevel.Super, Ticket.SuperDiscount },
                { UserLevel.Master, Ticket.MasterDiscount },
                { UserLevel.Agent, Ticket.AgentDiscount }
            };
    }

    private string GetCommisionPercentText(string commPercent)
        => Ticket.CommStatus == "1" ? $"2x {commPercent}%" : $"{commPercent}%";

    private bool BetStatusIsRunningOrWaiting()
    {
        var status = Ticket.Status.ToLowerInvariant();

        return status.Equals(BetStatus.Running) ||
            status.Equals(BetStatus.Waiting) ||
            status.Equals(BetStatus.Pending);
    }
}