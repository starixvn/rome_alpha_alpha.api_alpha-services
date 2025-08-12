using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Utilities.Extensions;

namespace Alpha.Reports.API.Helpers;

public static class WinLossDetailMapper
{
    public static GrpcReports.SuperWinLossDetailModel MapToGrpcModel(this Models.WinLossDetail.SuperWinLossDetailModel source)
    {
        return new GrpcReports.SuperWinLossDetailModel
        {
            CustName = source.CustName ?? string.Empty,
            BetCount = source.BetCount,
            Turnover = source.Turnover.FormatNumber(),
            MemberWinLoss = source.MemberWinLoss.FormatNumber(),
            MemberComm = source.MemberComm.FormatNumber(),
            MemberTotal = source.MemberTotal.FormatNumber(),
            AgentWinLoss = source.AgentWinLoss.FormatNumber(),
            AgentComm = source.AgentComm.FormatNumber(),
            AgentTotal = source.AgentTotal.FormatNumber(),
            MasterWinLoss = source.MasterWinLoss.FormatNumber(),
            MasterComm = source.MasterComm.FormatNumber(),
            MasterTotal = source.MasterTotal.FormatNumber(),
            SuperWinLoss = source.SuperWinLoss.FormatNumber(),
            SuperComm = source.SuperComm.FormatNumber(),
            SuperTotal = source.SuperTotal.FormatNumber(),
            Company = source.Company.FormatNumber(),
            NetTurnover = source.NetTurnover.FormatNumber(),
            GrossComm = source.GrossComm.FormatNumber(),
            RewardAmount = source.RewardAmount.FormatNumber(),
            RewardCurrency = source.RewardCurrency ?? string.Empty,
            IsDirectMember = source.IsDirectMember
        };
    }

    public static GrpcReports.MasterWinLossDetailModel MapToGrpcModel(this Models.WinLossDetail.MasterWinLossDetailModel source)
    {
        return new GrpcReports.MasterWinLossDetailModel
        {
            CustName = source.CustName ?? string.Empty,
            BetCount = source.BetCount,
            Turnover = source.Turnover.FormatNumber(),
            MemberWinLoss = source.MemberWinLoss.FormatNumber(),
            MemberComm = source.MemberComm.FormatNumber(),
            MemberTotal = source.MemberTotal.FormatNumber(),
            AgentWinLoss = source.AgentWinLoss.FormatNumber(),
            AgentComm = source.AgentComm.FormatNumber(),
            AgentTotal = source.AgentTotal.FormatNumber(),
            MasterWinLoss = source.MasterWinLoss.FormatNumber(),
            MasterComm = source.MasterComm.FormatNumber(),
            MasterTotal = source.MasterTotal.FormatNumber(),
            Company = source.Company.FormatNumber(),
            NetTurnover = source.NetTurnover.FormatNumber(),
            GrossComm = source.GrossComm.FormatNumber(),
            RewardAmount = source.RewardAmount.FormatNumber(),
            RewardCurrency = source.RewardCurrency ?? string.Empty,
            IsDirectMember = source.IsDirectMember
        };
    }

    public static GrpcReports.AgentWinLossDetailModel MapToGrpcModel(this Models.WinLossDetail.AgentWinLossDetailModel source)
    {
        return new GrpcReports.AgentWinLossDetailModel
        {
            CustName = source.CustName ?? string.Empty,
            BetCount = source.BetCount,
            Turnover = source.Turnover.FormatNumber(),
            MemberWinLoss = source.MemberWinLoss.FormatNumber(),
            MemberComm = source.MemberComm.FormatNumber(),
            MemberTotal = source.MemberTotal.FormatNumber(),
            AgentWinLoss = source.AgentWinLoss.FormatNumber(),
            AgentComm = source.AgentComm.FormatNumber(),
            AgentTotal = source.AgentTotal.FormatNumber(),
            Company = source.Company.FormatNumber(),
            NetTurnover = source.NetTurnover.FormatNumber(),
            GrossComm = source.GrossComm.FormatNumber(),
            RewardAmount = source.RewardAmount.FormatNumber(),
            RewardCurrency = source.RewardCurrency ?? string.Empty
        };
    }
}
