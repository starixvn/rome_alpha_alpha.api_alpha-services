namespace Alpha.Reports.API.Models.WinLossDetail;

public class AgentWinLossDetailModel
{
    public int CustId { get; set; }

    public string CustName { get; set; }

    public int BetCount { get; set; }

    public decimal Turnover { get; set; }

    public decimal NetTurnover { get; set; }

    public decimal GrossComm { get; set; }

    public decimal RewardAmount { get; set; }

    public string RewardCurrency { get; set; } = string.Empty;

    public decimal MemberWinLoss { get; set; }

    public decimal MemberComm { get; set; }

    public decimal MemberTotal => MemberWinLoss + MemberComm;

    public decimal AgentWinLoss { get; set; }

    public decimal AgentComm { get; set; }

    public decimal AgentTotal => AgentWinLoss + AgentComm;

    public virtual decimal Company => -(AgentTotal + MemberTotal);    
}