using Alpha.Reports.API.Extensions;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Services;
using Grpc.Core;

namespace GrpcReports;

public class ReportGrpcService : Reports.ReportsBase
{
    private readonly IProductService productService;
    private readonly IOutstandingService outstandingService;

    public ReportGrpcService(
        IProductService productService,
        IOutstandingService outstandingService)
    {
        this.productService = productService;
        this.outstandingService = outstandingService;
    }

    public override async Task<GetOutstandingReportResponse> GetOutstandingReport(
        GetOutstandingReportRequest request,
        ServerCallContext context)
    {
        var validUserSelectedProductIds = productService.FilterProductIdsByCustomer(
            request.ViewerId,
            request.Site,
            request.CurrencyId,
            Enumerable.Empty<int>(),
            DateTime.Now);

        var param = new GetOutstandingReportParam(
            (byte)request.ViewerLevelId,
            (byte)request.CustLevelId,
            request.CustId,
            validUserSelectedProductIds.JoinWithComma(),
            request.AutoFollowBetIsDummyAgent);

        var outstandings = (await outstandingService.GetReport(param))
            .OrderBy(request.SortPropertyName, request.IsDescendingSort);

        var response = new GetOutstandingReportResponse();
        response.Report.AddRange(outstandings.Select(o => new OutstandingModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            AgentOutstanding = request.ViewerLevelId >= UserLevelModel.Agent ? (float)o.AgentOutstanding : 0,
            MasterOutstanding = request.ViewerLevelId >= UserLevelModel.Master ? (float)o.MasterOutstanding : 0,
            SuperOutstanding = request.ViewerLevelId >= UserLevelModel.Super ? (float)o.SuperOutstanding : 0,
        }));

        return response;
    }
}