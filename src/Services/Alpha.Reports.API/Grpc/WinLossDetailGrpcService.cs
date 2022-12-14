
using Alpha.Reports.API.Params;
using Alpha.Utilities.Extensions;
using Grpc.Core;

namespace GrpcReports;

public partial class ReportGrpcService : Reports.ReportsBase
{
    public override async Task<GetSuperWinLossDetailResponse> GetSuperWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var products = productService.GetProductsForCustomer(param.ViewerId, param.Site, param.CurrencyId, param.Language, param.MinDate, param.IsEnableSabaPromotion);
        var reportData = await winLossDetailService.GetSuperWinLossDetail(param);

        var response = new GetSuperWinLossDetailResponse
        {
            Total = new SuperWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = reportData.TotalData.Turnover.FormatNumber(),
                MemberWinLoss = reportData.TotalData.MemberWinLoss.FormatNumber(),
                MemberComm = reportData.TotalData.MemberComm.FormatNumber(),
                AgentWinLoss = reportData.TotalData.AgentWinLoss.FormatNumber(),
                AgentComm = reportData.TotalData.AgentComm.FormatNumber(),
                MasterWinLoss = reportData.TotalData.MasterWinLoss.FormatNumber(),
                MasterComm = reportData.TotalData.MasterComm.FormatNumber(),
                SuperWinLoss = reportData.TotalData.SuperWinLoss.FormatNumber(),
                SuperComm = reportData.TotalData.SuperComm.FormatNumber(),
                Company = reportData.TotalData.Company.FormatNumber()
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new SuperWinLossDetailModel
        {
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = o.Turnover.FormatNumber(),
            MemberWinLoss = o.MemberWinLoss.FormatNumber(),
            MemberComm = o.MemberComm.FormatNumber(),
            AgentWinLoss = o.AgentWinLoss.FormatNumber(),
            AgentComm = o.AgentComm.FormatNumber(),
            MasterWinLoss = o.MasterWinLoss.FormatNumber(),
            MasterComm = o.MasterComm.FormatNumber(),
            SuperWinLoss = o.SuperWinLoss.FormatNumber(),
            SuperComm = o.SuperComm.FormatNumber(),
            Company = o.Company.FormatNumber()
        }));

        return response;
    }

    public override async Task<GetMasterWinLossDetailResponse> GetMasterWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var products = productService.GetProductsForCustomer(param.ViewerId, param.Site, param.CurrencyId, param.Language, param.MinDate, param.IsEnableSabaPromotion);
        var reportData = await winLossDetailService.GetMasterWinLossDetail(param);

        var response = new GetMasterWinLossDetailResponse
        {
            Total = new MasterWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = reportData.TotalData.Turnover.FormatNumber(),
                MemberWinLoss = reportData.TotalData.MemberWinLoss.FormatNumber(),
                MemberComm = reportData.TotalData.MemberComm.FormatNumber(),
                AgentWinLoss = reportData.TotalData.AgentWinLoss.FormatNumber(),
                AgentComm = reportData.TotalData.AgentComm.FormatNumber(),
                MasterWinLoss = reportData.TotalData.MasterWinLoss.FormatNumber(),
                MasterComm = reportData.TotalData.MasterComm.FormatNumber(),
                Company = reportData.TotalData.Company.FormatNumber()
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new MasterWinLossDetailModel
        {
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = o.Turnover.FormatNumber(),
            MemberWinLoss = o.MemberWinLoss.FormatNumber(),
            MemberComm = o.MemberComm.FormatNumber(),
            AgentWinLoss = o.AgentWinLoss.FormatNumber(),
            AgentComm = o.AgentComm.FormatNumber(),
            MasterWinLoss = o.MasterWinLoss.FormatNumber(),
            MasterComm = o.MasterComm.FormatNumber(),
            Company = o.Company.FormatNumber()
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
                AgentWinLoss = reportData.TotalData.AgentWinLoss.FormatNumber(),
                AgentComm = reportData.TotalData.AgentComm.FormatNumber(),
                Company = reportData.TotalData.Company.FormatNumber()
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new AgentWinLossDetailModel
        {
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = o.Turnover.FormatNumber(),
            MemberWinLoss = o.MemberWinLoss.FormatNumber(),
            MemberComm = o.MemberComm.FormatNumber(),
            AgentWinLoss = o.AgentWinLoss.FormatNumber(),
            AgentComm = o.AgentComm.FormatNumber(),
            Company = o.Company.FormatNumber()
        }));

        return response;
    }
}