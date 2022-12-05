using Alpha.Reports.API.Extensions;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Services;
using Grpc.Core;
using Alpha.Reports.API.Models;

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

    public override async Task<GetSuperOutstandingResponse> GetSuperOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetSuperOutstandingResponse();
        response.Total = new SuperOutstandingModel
        {
            MemberOutstanding = (float)total.MemberOutstanding,
            SuperOutstanding = (float)total.SuperOutstanding
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOutstandingModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            SuperOutstanding = (float)o.SuperOutstanding
        }));

        return response;
    }

    public override async Task<GetSuperOnMasterOutstandingResponse> GetSuperOnMasterOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetSuperOnMasterOutstandingResponse();
        response.Total = new SuperOnMasterOutstandingModel
        {
            MemberOutstanding = (float)total.MemberOutstanding,
            MasterOutstanding = (float)total.MasterOutstanding,
            SuperOutstanding = (float)total.SuperOutstanding
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOnMasterOutstandingModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            MasterOutstanding = (float)o.MasterOutstanding,
            SuperOutstanding = (float)o.SuperOutstanding
        }));

        return response;
    }

    public override async Task<GetSuperOnAgentOutstandingResponse> GetSuperOnAgentOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetSuperOnAgentOutstandingResponse();
        response.Total = new SuperOnAgentOutstandingModel
        {
            MemberOutstanding = (float)total.MemberOutstanding,
            AgentOutstanding = (float)total.AgentOutstanding,
            MasterOutstanding = (float)total.MasterOutstanding,
            SuperOutstanding = (float)total.SuperOutstanding
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOnAgentOutstandingModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            AgentOutstanding = (float)o.AgentOutstanding,
            MasterOutstanding = (float)o.MasterOutstanding,
            SuperOutstanding = (float)o.SuperOutstanding
        }));

        return response;
    }

    public override async Task<GetMasterOutstandingResponse> GetMasterOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetMasterOutstandingResponse();
        response.Total = new MasterOutstandingModel
        {
            MemberOutstanding = (float)total.MemberOutstanding,
            MasterOutstanding = (float)total.MasterOutstanding,
        };

        response.Report.AddRange(outstandings.Select(o => new MasterOutstandingModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            MasterOutstanding = (float)o.MasterOutstanding
        }));

        return response;
    }

    public override async Task<GetMasterOnAgentOutstandingResponse> GetMasterOnAgentOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetMasterOnAgentOutstandingResponse();
        response.Total = new MasterOnAgentOutstandingModel
        {
            MemberOutstanding = (float)total.MemberOutstanding,
            AgentOutstanding = (float)total.AgentOutstanding,
            MasterOutstanding = (float)total.MasterOutstanding
        };

        response.Report.AddRange(outstandings.Select(o => new MasterOnAgentOutstandingModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            AgentOutstanding = (float)total.AgentOutstanding,
            MasterOutstanding = (float)o.MasterOutstanding
        }));

        return response;
    }

    public override async Task<GetAgentOutstandingResponse> GetAgentOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetAgentOutstandingResponse();
        response.Total = new AgentOutstandingModel
        {
            MemberOutstanding = (float)total.MemberOutstanding,
            AgentOutstanding = (float)total.AgentOutstanding
        };

        response.Report.AddRange(outstandings.Select(o => new AgentOutstandingModel
        {
            CustId = o.CustId,
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            AgentOutstanding = (float)total.AgentOutstanding
        }));

        return response;
    }

    private async Task<IEnumerable<OutstandingModel>> GetOutstanding(GetOutstandingReportRequest request)
    {
        var validUserSelectedProductIds = await productService.FilterProductIdsByCustomer(
            request.ViewerId,
            request.Site,
            request.CurrencyId,
            request.Language,
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

        return outstandings;
    }
}