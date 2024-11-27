namespace Alpha.Reports.API.Models.WinLossDetail;

public class SuperWinLossDetailModel : MasterWinLossDetailModel
{
    public decimal SuperWinLoss { get; set; }

    public decimal SuperComm { get; set; }

    public decimal SuperTotal => SuperWinLoss + SuperComm;

    public override decimal Company => -(SuperTotal + MasterTotal + AgentTotal + MemberTotal);
}