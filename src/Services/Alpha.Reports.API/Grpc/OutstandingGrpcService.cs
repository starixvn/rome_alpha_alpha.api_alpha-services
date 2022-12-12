using Alpha.Reports.API.Extensions;
using Alpha.Reports.API.Params;
using Grpc.Core;
using Alpha.Reports.API.Models;

namespace GrpcReports;

public partial class ReportGrpcService : Reports.ReportsBase
{
    public override async Task<GetSuperOutstandingResponse> GetSuperOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetSuperOutstandingResponse
        {
            Total = new SuperOutstandingModel
            {
                MemberOutstanding = (float)total.MemberOutstanding,
                SuperOutstanding = (float)total.SuperOutstanding
            }
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOutstandingModel
        {
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

        var response = new GetSuperOnMasterOutstandingResponse
        {
            Total = new SuperOnMasterOutstandingModel
            {
                MemberOutstanding = (float)total.MemberOutstanding,
                MasterOutstanding = (float)total.MasterOutstanding,
                SuperOutstanding = (float)total.SuperOutstanding
            }
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOnMasterOutstandingModel
        {
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

        var response = new GetSuperOnAgentOutstandingResponse
        {
            Total = new SuperOnAgentOutstandingModel
            {
                MemberOutstanding = (float)total.MemberOutstanding,
                AgentOutstanding = (float)total.AgentOutstanding,
                MasterOutstanding = (float)total.MasterOutstanding,
                SuperOutstanding = (float)total.SuperOutstanding
            }
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOnAgentOutstandingModel
        {
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

        var response = new GetMasterOutstandingResponse
        {
            Total = new MasterOutstandingModel
            {
                MemberOutstanding = (float)total.MemberOutstanding,
                MasterOutstanding = (float)total.MasterOutstanding,
            }
        };

        response.Report.AddRange(outstandings.Select(o => new MasterOutstandingModel
        {
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

        var response = new GetMasterOnAgentOutstandingResponse
        {
            Total = new MasterOnAgentOutstandingModel
            {
                MemberOutstanding = (float)total.MemberOutstanding,
                AgentOutstanding = (float)total.AgentOutstanding,
                MasterOutstanding = (float)total.MasterOutstanding
            }
        };

        response.Report.AddRange(outstandings.Select(o => new MasterOnAgentOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            AgentOutstanding = (float)o.AgentOutstanding,
            MasterOutstanding = (float)o.MasterOutstanding
        }));

        return response;
    }

    public override async Task<GetAgentOutstandingResponse> GetAgentOutstanding(GetOutstandingReportRequest request, ServerCallContext context)
    {
        var outstandings = await GetOutstanding(request);
        var total = OutstandingModel.GetAccumulatedData(outstandings);

        var response = new GetAgentOutstandingResponse
        {
            Total = new AgentOutstandingModel
            {
                MemberOutstanding = (float)total.MemberOutstanding,
                AgentOutstanding = (float)total.AgentOutstanding
            }
        };

        response.Report.AddRange(outstandings.Select(o => new AgentOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = (float)o.MemberOutstanding,
            AgentOutstanding = (float)o.AgentOutstanding
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