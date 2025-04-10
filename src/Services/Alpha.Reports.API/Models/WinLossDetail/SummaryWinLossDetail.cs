namespace Alpha.Reports.API.Models.WinLossDetail;

public class SummaryWinLossDetail : SuperWinLossDetailModel
{
    public SummaryWinLossDetail(SuperWinLossDetailModel super) : this((MasterWinLossDetailModel)super)
    {
        SuperWinLoss = super.SuperWinLoss;
        SuperComm = super.SuperComm;
        DownlineTotal += super.MasterTotal;
    }

    public SummaryWinLossDetail(MasterWinLossDetailModel master) : this((AgentWinLossDetailModel)master)
    {
        MasterWinLoss = master.MasterWinLoss;
        MasterComm = master.MasterComm;
        DownlineTotal = master.AgentTotal + master.MemberTotal;
    }

    public SummaryWinLossDetail(AgentWinLossDetailModel agent)
    {
        BetCount = agent.BetCount;
        Turnover = agent.Turnover;
        NetTurnover = agent.NetTurnover;
        GrossComm = agent.GrossComm;
        MemberWinLoss = agent.MemberWinLoss;
        MemberComm = agent.MemberComm;
        AgentWinLoss = agent.AgentWinLoss;
        AgentComm = agent.AgentComm;
    }

    public decimal DownlineTotal { get; }

    public string ProductIds { get; set; }

    public string ProductDetailIds { get; set; }
}
