using Alpha.Reports.API.Extensions;
using Alpha.Reports.API.Params;
using Grpc.Core;
using Alpha.Reports.API.Models;
using Alpha.Utilities.Extensions;

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
                MemberOutstanding = total.MemberOutstanding.FormatNumber(),
                SuperOutstanding = total.SuperOutstanding.FormatNumber()
            }
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = o.MemberOutstanding.FormatNumber(),
            SuperOutstanding = o.SuperOutstanding.FormatNumber()
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
                MemberOutstanding = total.MemberOutstanding.FormatNumber(),
                MasterOutstanding = total.MasterOutstanding.FormatNumber(),
                SuperOutstanding = total.SuperOutstanding.FormatNumber()
            }
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOnMasterOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = o.MemberOutstanding.FormatNumber(),
            MasterOutstanding = o.MasterOutstanding.FormatNumber(),
            SuperOutstanding = o.SuperOutstanding.FormatNumber()
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
                MemberOutstanding = total.MemberOutstanding.FormatNumber(),
                AgentOutstanding = total.AgentOutstanding.FormatNumber(),
                MasterOutstanding = total.MasterOutstanding.FormatNumber(),
                SuperOutstanding = total.SuperOutstanding.FormatNumber()
            }
        };

        response.Report.AddRange(outstandings.Select(o => new SuperOnAgentOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = o.MemberOutstanding.FormatNumber(),
            AgentOutstanding = o.AgentOutstanding.FormatNumber(),
            MasterOutstanding = o.MasterOutstanding.FormatNumber(),
            SuperOutstanding = o.SuperOutstanding.FormatNumber()
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
                MemberOutstanding = total.MemberOutstanding.FormatNumber(),
                MasterOutstanding = total.MasterOutstanding.FormatNumber(),
            }
        };

        response.Report.AddRange(outstandings.Select(o => new MasterOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = o.MemberOutstanding.FormatNumber(),
            MasterOutstanding = o.MasterOutstanding.FormatNumber()
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
                MemberOutstanding = total.MemberOutstanding.FormatNumber(),
                AgentOutstanding = total.AgentOutstanding.FormatNumber(),
                MasterOutstanding = total.MasterOutstanding.FormatNumber()
            }
        };

        response.Report.AddRange(outstandings.Select(o => new MasterOnAgentOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = o.MemberOutstanding.FormatNumber(),
            AgentOutstanding = o.AgentOutstanding.FormatNumber(),
            MasterOutstanding = o.MasterOutstanding.FormatNumber()
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
                MemberOutstanding = total.MemberOutstanding.FormatNumber(),
                AgentOutstanding = total.AgentOutstanding.FormatNumber()
            }
        };

        response.Report.AddRange(outstandings.Select(o => new AgentOutstandingModel
        {
            CustName = o.CustName,
            MemberOutstanding = o.MemberOutstanding.FormatNumber(),
            AgentOutstanding = o.AgentOutstanding.FormatNumber()
        }));

        return response;
    }

    private async Task<IEnumerable<OutstandingModel>> GetOutstanding(GetOutstandingReportRequest request)
    {
        var validUserSelectedProductIds = await productService.FilterProductIdsByCustomer(
            request.ViewerId,
            request.ViewerLevelId,
            request.Site,
            request.CurrencyId,
            request.Language,
            Enumerable.Empty<int>(),
            DateTime.Now);

        validUserSelectedProductIds = productService.FilterProductIdsByGroup(validUserSelectedProductIds, request.ProductGroup);

        var param = new GetOutstandingReportParam(
            (byte)request.ViewerLevelId,
            (byte)request.CustLevelId,
            request.CustId,
            validUserSelectedProductIds.JoinWithComma(),
            request.AutoFollowBetIsDummyAgent,
            request.Site);

        var outstandings = (await outstandingService.GetReport(param))
            .OrderBy(request.SortPropertyName, request.IsDescendingSort);

        return outstandings;
    }
}