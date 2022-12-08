namespace Alpha.Reports.API.Models.WinLossDetail;

public class MasterWinLossDetailModel : AgentWinLossDetailModel
{
    public decimal MasterWinLoss { get; set; }

    public decimal MasterComm { get; set; }

    public decimal MasterTotal => MasterWinLoss + MasterComm;

    public new decimal Company => MasterTotal + AgentTotal - MemberTotal;
}