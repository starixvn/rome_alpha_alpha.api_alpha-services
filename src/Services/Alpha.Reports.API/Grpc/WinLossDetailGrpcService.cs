
using Alpha.Reports.API.Params;
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
            PageSize = param.PageSize,
            PageIndex = param.PageIndex,
            TotalRecords = reportData.TotalRecords,
            Total = new SuperWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = (float)reportData.TotalData.Turnover,
                MemberWinLoss = (float)reportData.TotalData.MemberWinLoss,
                MemberComm = (float)reportData.TotalData.MemberComm,
                AgentWinLoss = (float)reportData.TotalData.AgentWinLoss,
                AgentComm = (float)reportData.TotalData.AgentComm,
                MasterWinLoss = (float)reportData.TotalData.MasterWinLoss,
                MasterComm = (float)reportData.TotalData.MasterComm,
                SuperWinLoss = (float)reportData.TotalData.SuperWinLoss,
                SuperComm = (float)reportData.TotalData.SuperComm
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new SuperWinLossDetailModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = (float)o.Turnover,
            MemberWinLoss = (float)o.MemberWinLoss,
            MemberComm = (float)o.MemberComm,
            AgentWinLoss = (float)o.AgentWinLoss,
            AgentComm = (float)o.AgentComm,
            MasterWinLoss = (float)o.MasterWinLoss,
            MasterComm = (float)o.MasterComm,
            SuperWinLoss = (float)o.SuperWinLoss,
            SuperComm = (float)o.SuperComm
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
            PageSize = param.PageSize,
            PageIndex = param.PageIndex,
            TotalRecords = reportData.TotalRecords,
            Total = new MasterWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = (float)reportData.TotalData.Turnover,
                MemberWinLoss = (float)reportData.TotalData.MemberWinLoss,
                MemberComm = (float)reportData.TotalData.MemberComm,
                AgentWinLoss = (float)reportData.TotalData.AgentWinLoss,
                AgentComm = (float)reportData.TotalData.AgentComm,
                MasterWinLoss = (float)reportData.TotalData.MasterWinLoss,
                MasterComm = (float)reportData.TotalData.MasterComm
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new MasterWinLossDetailModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = (float)o.Turnover,
            MemberWinLoss = (float)o.MemberWinLoss,
            MemberComm = (float)o.MemberComm,
            AgentWinLoss = (float)o.AgentWinLoss,
            AgentComm = (float)o.AgentComm,
            MasterWinLoss = (float)o.MasterWinLoss,
            MasterComm = (float)o.MasterComm
        }));

        return response;
    }

    public override async Task<GetAgentWinLossDetailResponse> GetAgentWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var reportData = await winLossDetailService.GetAgentWinLossDetail(param);

        var response = new GetAgentWinLossDetailResponse
        {
            PageSize = param.PageSize,
            PageIndex = param.PageIndex,
            TotalRecords = reportData.TotalRecords,
            Total = new AgentWinLossDetailModel
            {
                BetCount = reportData.TotalData.BetCount,
                Turnover = (float)reportData.TotalData.Turnover,
                MemberWinLoss = (float)reportData.TotalData.MemberWinLoss,
                MemberComm = (float)reportData.TotalData.MemberComm,
                AgentWinLoss = (float)reportData.TotalData.AgentWinLoss,
                AgentComm = (float)reportData.TotalData.AgentComm
            }
        };

        response.Report.AddRange(reportData.DataSource.Select(o => new AgentWinLossDetailModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            BetCount = o.BetCount,
            Turnover = (float)o.Turnover,
            MemberWinLoss = (float)o.MemberWinLoss,
            MemberComm = (float)o.MemberComm,
            AgentWinLoss = (float)o.AgentWinLoss,
            AgentComm = (float)o.AgentComm
        }));

        return response;
    }
}