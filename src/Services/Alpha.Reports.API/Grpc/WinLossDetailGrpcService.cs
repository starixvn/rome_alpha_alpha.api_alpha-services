
using Alpha.Reports.API.Helpers;
using Alpha.Reports.API.Params;
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
            Total = reportData.TotalData.MapToGrpcModel()
        };

        response.Report.AddRange(reportData.DataSource.Select(o => o.MapToGrpcModel()));

        return response;
    }

    public override async Task<GetMasterWinLossDetailResponse> GetMasterWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var reportData = await winLossDetailService.GetMasterWinLossDetail(param);

        var response = new GetMasterWinLossDetailResponse
        {
            Total = reportData.TotalData.MapToGrpcModel()
        };

        response.Report.AddRange(reportData.DataSource.Select(o => o.MapToGrpcModel()));

        return response;
    }

    public override async Task<GetAgentWinLossDetailResponse> GetAgentWinLossDetail(GetWinLossDetailReportRequest request, ServerCallContext context)
    {
        var param = new GetWinLossDetailReportParam(request, settings);
        var reportData = await winLossDetailService.GetAgentWinLossDetail(param);

        var response = new GetAgentWinLossDetailResponse
        {
            Total = reportData.TotalData.MapToGrpcModel()
        };

        response.Report.AddRange(reportData.DataSource.Select(o => o.MapToGrpcModel()));

        return response;
    }
}