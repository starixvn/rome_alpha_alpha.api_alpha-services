
using Alpha.Reports.API.Params;
using Alpha.Utilities.Extensions;
using Grpc.Core;

namespace GrpcReports;

public partial class ReportGrpcService : Reports.ReportsBase
{
    public override async Task<GetSuperWinLossDetailResponse> GetSuperWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var reportData = await winLossDetailService.GetSuperWinLossDetail(param);

        var response = new GetSuperWinLossDetailResponse
        {
            Total = new SuperWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = reportData.TotalData.Turnover.FormatNumber(),
                MemberWinLoss = reportData.TotalData.MemberWinLoss.FormatNumber(),
                MemberComm = reportData.TotalData.MemberComm.FormatNumber(),
                MemberTotal = reportData.TotalData.MemberTotal.FormatNumber(),
                AgentWinLoss = reportData.TotalData.AgentWinLoss.FormatNumber(),
                AgentComm = reportData.TotalData.AgentComm.FormatNumber(),
                AgentTotal = reportData.TotalData.AgentTotal.FormatNumber(),
                MasterWinLoss = reportData.TotalData.MasterWinLoss.FormatNumber(),
                MasterComm = reportData.TotalData.MasterComm.FormatNumber(),
                MasterTotal = reportData.TotalData.MasterTotal.FormatNumber(),
                SuperWinLoss = reportData.TotalData.SuperWinLoss.FormatNumber(),
                SuperComm = reportData.TotalData.SuperComm.FormatNumber(),
                SuperTotal = reportData.TotalData.SuperTotal.FormatNumber(),
                Company = reportData.TotalData.Company.FormatNumber(),
                NetTurnover = reportData.TotalData.NetTurnover.FormatNumber(),
                GrossComm = reportData.TotalData.GrossComm.FormatNumber(),
                RewardAmount = reportData.TotalData.RewardAmount.FormatNumber(),
                RewardCurrency = reportData.TotalData.RewardCurrency
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new SuperWinLossDetailModel
        {
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = o.Turnover.FormatNumber(),
            MemberWinLoss = o.MemberWinLoss.FormatNumber(),
            MemberComm = o.MemberComm.FormatNumber(),
            MemberTotal = o.MemberTotal.FormatNumber(),
            AgentWinLoss = o.AgentWinLoss.FormatNumber(),
            AgentComm = o.AgentComm.FormatNumber(),
            AgentTotal = o.AgentTotal.FormatNumber(),
            MasterWinLoss = o.MasterWinLoss.FormatNumber(),
            MasterComm = o.MasterComm.FormatNumber(),
            MasterTotal = o.MasterTotal.FormatNumber(),
            SuperWinLoss = o.SuperWinLoss.FormatNumber(),
            SuperComm = o.SuperComm.FormatNumber(),
            SuperTotal = o.SuperTotal.FormatNumber(),
            Company = o.Company.FormatNumber(),
            NetTurnover = o.NetTurnover.FormatNumber(),
            GrossComm = o.GrossComm.FormatNumber(),
            RewardAmount = o.RewardAmount.FormatNumber(),
            RewardCurrency = o.RewardCurrency,
            IsDirectMember = o.IsDirectMember
        }));

        return response;
    }

    public override async Task<GetMasterWinLossDetailResponse> GetMasterWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var reportData = await winLossDetailService.GetMasterWinLossDetail(param);

        var response = new GetMasterWinLossDetailResponse
        {
            Total = new MasterWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = reportData.TotalData.Turnover.FormatNumber(),
                MemberWinLoss = reportData.TotalData.MemberWinLoss.FormatNumber(),
                MemberComm = reportData.TotalData.MemberComm.FormatNumber(),
                MemberTotal = reportData.TotalData.MemberTotal.FormatNumber(),
                AgentWinLoss = reportData.TotalData.AgentWinLoss.FormatNumber(),
                AgentComm = reportData.TotalData.AgentComm.FormatNumber(),
                AgentTotal = reportData.TotalData.AgentTotal.FormatNumber(),
                MasterWinLoss = reportData.TotalData.MasterWinLoss.FormatNumber(),
                MasterComm = reportData.TotalData.MasterComm.FormatNumber(),
                MasterTotal = reportData.TotalData.MasterTotal.FormatNumber(),
                Company = reportData.TotalData.Company.FormatNumber(),
                NetTurnover = reportData.TotalData.NetTurnover.FormatNumber(),
                GrossComm = reportData.TotalData.GrossComm.FormatNumber(),
                RewardAmount = reportData.TotalData.RewardAmount.FormatNumber(),
                RewardCurrency = reportData.TotalData.RewardCurrency
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new MasterWinLossDetailModel
        {
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = o.Turnover.FormatNumber(),
            MemberWinLoss = o.MemberWinLoss.FormatNumber(),
            MemberComm = o.MemberComm.FormatNumber(),
            MemberTotal = o.MemberTotal.FormatNumber(),
            AgentWinLoss = o.AgentWinLoss.FormatNumber(),
            AgentComm = o.AgentComm.FormatNumber(),
            AgentTotal = o.AgentTotal.FormatNumber(),
            MasterWinLoss = o.MasterWinLoss.FormatNumber(),
            MasterComm = o.MasterComm.FormatNumber(),
            MasterTotal = o.MasterTotal.FormatNumber(),
            Company = o.Company.FormatNumber(),
            NetTurnover = o.NetTurnover.FormatNumber(),
            GrossComm = o.GrossComm.FormatNumber(),
            RewardAmount = o.RewardAmount.FormatNumber(),
            RewardCurrency = o.RewardCurrency,
            IsDirectMember = o.IsDirectMember
        }));

        return response;
    }

    public override async Task<GetAgentWinLossDetailResponse> GetAgentWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var reportData = await winLossDetailService.GetAgentWinLossDetail(param);

        var response = new GetAgentWinLossDetailResponse
        {
            Total = new AgentWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = reportData.TotalData.Turnover.FormatNumber(),
                MemberWinLoss = reportData.TotalData.MemberWinLoss.FormatNumber(),
                MemberComm = reportData.TotalData.MemberComm.FormatNumber(),
                MemberTotal = reportData.TotalData.MemberTotal.FormatNumber(),
                AgentWinLoss = reportData.TotalData.AgentWinLoss.FormatNumber(),
                AgentComm = reportData.TotalData.AgentComm.FormatNumber(),
                AgentTotal = reportData.TotalData.AgentTotal.FormatNumber(),
                Company = reportData.TotalData.Company.FormatNumber(),
                NetTurnover = reportData.TotalData.NetTurnover.FormatNumber(),
                GrossComm = reportData.TotalData.GrossComm.FormatNumber(),
                RewardAmount = reportData.TotalData.RewardAmount.FormatNumber(),
                RewardCurrency = reportData.TotalData.RewardCurrency
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new AgentWinLossDetailModel
        {
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = o.Turnover.FormatNumber(),
            MemberWinLoss = o.MemberWinLoss.FormatNumber(),
            MemberComm = o.MemberComm.FormatNumber(),
            MemberTotal = o.MemberTotal.FormatNumber(),
            AgentWinLoss = o.AgentWinLoss.FormatNumber(),
            AgentComm = o.AgentComm.FormatNumber(),
            AgentTotal = o.AgentTotal.FormatNumber(),
            Company = o.Company.FormatNumber(),
            NetTurnover = o.NetTurnover.FormatNumber(),
            GrossComm = o.GrossComm.FormatNumber(),
            RewardAmount = o.RewardAmount.FormatNumber(),
            RewardCurrency = o.RewardCurrency
        }));

        return response;
    }
}