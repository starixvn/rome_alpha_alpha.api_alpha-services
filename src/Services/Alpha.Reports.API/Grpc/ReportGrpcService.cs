using Grpc.Core;

namespace GrpcReports;

public class ReportGrpcService : Reports.ReportsBase
{

    public ReportGrpcService() {}

    public override async Task<GetOutstandingReportResponse> GetOutstandingReport(GetOutstandingReportRequest request, ServerCallContext context)
    {
        return new GetOutstandingReportResponse();
    }
}