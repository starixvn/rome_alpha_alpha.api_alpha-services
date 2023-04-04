namespace Alpha.Reports.API.Models.WinLossDetail;

public class AgentWinLossDetailModel
{
    public int CustId { get; set; }

    public string CustName { get; set; }

    public int BetCount { get; set; }

    public decimal Turnover { get; set; }

    public decimal MemberWinLoss { get; set; }

    public decimal MemberComm { get; set; }

    public decimal AgentWinLoss { get; set; }

    public decimal AgentComm { get; set; }

    public decimal MemberTotal => MemberWinLoss + MemberComm;

    public decimal AgentTotal => AgentWinLoss + AgentComm;

    public decimal Company => -(AgentTotal + MemberTotal);
}