using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Utilities.Extensions;

namespace Alpha.Reports.API.Helpers;

public static class WinLossDetailMapper
{
    public static GrpcReports.SuperWinLossDetailModel MapToGrpcModel(this SuperWinLossDetailModel source)
    {
        var grpcModel = new GrpcReports.SuperWinLossDetailModel();
        MapBaseProperties(source, grpcModel);
        MapMasterProperties(source, grpcModel);
        MapSuperProperties(source, grpcModel);
        return grpcModel;
    }

    public static GrpcReports.MasterWinLossDetailModel MapToGrpcModel(this MasterWinLossDetailModel source)
    {
        var grpcModel = new GrpcReports.MasterWinLossDetailModel();
        MapBaseProperties(source, grpcModel);
        MapMasterProperties(source, grpcModel);
        return grpcModel;
    }

    public static GrpcReports.AgentWinLossDetailModel MapToGrpcModel(this AgentWinLossDetailModel source)
    {
        var grpcModel = new GrpcReports.AgentWinLossDetailModel();
        MapBaseProperties(source, grpcModel);
        return grpcModel;
    }

    private static void MapBaseProperties(AgentWinLossDetailModel source, dynamic target)
    {
        target.CustName = source.CustName ?? string.Empty;
        target.BetCount = source.BetCount;
        target.Turnover = source.Turnover.FormatNumber();
        target.MemberWinLoss = source.MemberWinLoss.FormatNumber();
        target.MemberComm = source.MemberComm.FormatNumber();
        target.MemberTotal = source.MemberTotal.FormatNumber();
        target.AgentWinLoss = source.AgentWinLoss.FormatNumber();
        target.AgentComm = source.AgentComm.FormatNumber();
        target.AgentTotal = source.AgentTotal.FormatNumber();
        target.Company = source.Company.FormatNumber();
        target.NetTurnover = source.NetTurnover.FormatNumber();
        target.GrossComm = source.GrossComm.FormatNumber();
        target.RewardAmount = source.RewardAmount.FormatNumber();
        target.RewardCurrency = source.RewardCurrency ?? string.Empty;
    }

    private static void MapMasterProperties(MasterWinLossDetailModel source, dynamic target)
    {
        target.MasterWinLoss = source.MasterWinLoss.FormatNumber();
        target.MasterComm = source.MasterComm.FormatNumber();
        target.MasterTotal = source.MasterTotal.FormatNumber();
        target.IsDirectMember = source.IsDirectMember;
    }

    private static void MapSuperProperties(SuperWinLossDetailModel source, dynamic target)
    {
        target.SuperWinLoss = source.SuperWinLoss.FormatNumber();
        target.SuperComm = source.SuperComm.FormatNumber();
        target.SuperTotal = source.SuperTotal.FormatNumber();
    }
}
