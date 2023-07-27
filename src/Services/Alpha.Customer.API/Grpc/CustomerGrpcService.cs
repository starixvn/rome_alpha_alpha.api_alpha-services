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
        logger.LogDebug("Begin grpc call from method {Method} for CustId {CustId} SubAccId {SubAccId}", context.Method, request.CustId, request.SubAccId);

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

    public override async Task<GetDownlineInfoResponse> GetDownlineInfo(GetDownlineInfoRequest request, ServerCallContext context)
    {
        var downlineInfo = await customerService.GetDownlineInfo(request.ViewerId, request.DownlineUsername);

        return new GetDownlineInfoResponse
        {
            IsBelonging = downlineInfo != null && downlineInfo.CustId > 0
        };
    }

    public override async Task<GetCustomerInfoResponse> GetCustomerInfo(GetCustomerInfoRequest request, ServerCallContext context)
    {
        var customerInfo = await customerService.GetCustomerInfo(request.CustId, request.Username);

        return new GetCustomerInfoResponse
        {
            CustId = customerInfo.CustId,
            RoleId = customerInfo.RoleId,
            Site = customerInfo.Site,
            Currency = customerInfo.Currency
        };
    }

    public override async Task<GetCustomerProductsResponse> GetCustomerProducts(GetCustomerProductsRequest request, ServerCallContext context)
    {
        var products = await customerService.GetCustomerProducts(request.CustId, request.RoleId);
        var response = new GetCustomerProductsResponse();
        response.Products.AddRange(products.Select(p => new ProductsResponse
        {
            Id = p.Id,
            IsSynced = p.IsSynced,
            IsHidden = p.IsHidden,
        }));

        return response;
    }
}