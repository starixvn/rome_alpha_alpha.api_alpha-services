using Alpha.Customer.API.Params;
using Alpha.Customer.API.Services;
using Grpc.Core;

namespace GrpcCustomer;

public class CustomerGrpcService : Customer.CustomerBase
{
    private readonly ICustomerService customerService;
    private readonly ILogger<CustomerGrpcService> logger;

    public CustomerGrpcService(ICustomerService customerService, ILogger<CustomerGrpcService> logger)
    {
        this.customerService = customerService;
        this.logger = logger;
    }

    public override async Task<GetCustomerStatusResponse> GetCustomerStatus(GetCustomerStatusRequest request, ServerCallContext context)
    {
        logger.LogInformation("Begin grpc call from method {Method} for CustId {CustId} SubAccId {SubAccId}", context.Method, request.CustId, request.SubAccId);

        var status = await customerService.GetCustomerStatus(new CustomerStatusParam(request.CustId, request.SubAccId));
        if (status != null)
        {
            context.Status = new Status(StatusCode.OK, $"Customer status with CustId {request.CustId} SubAccId {request.SubAccId} do exist");
            return new GetCustomerStatusResponse
            {
                CustId = status.CustId,
                SubAccId = status.SubAccId,
                IsActive = status.IsActive,
                IsProtectedMode = status.IsProtectedMode,
                IsLocked = status.IsLocked,
                IsSubAcc = status.IsSubAcc,
                HasReportPermission = status.HasReportPermission,
                HasMemberInformationPermission = status.HasMemberInformationPermission,
            };
        }

        context.Status = new Status(StatusCode.NotFound, $"Customer status with CustId {request.CustId} SubAccId {request.SubAccId} do not exist");

        return new GetCustomerStatusResponse();
    }
}