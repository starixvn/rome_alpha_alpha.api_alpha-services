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
        if (status == null)
        {
            context.Status = new Status(StatusCode.NotFound, $"Customer status with CustId {request.CustId} SubAccId {request.SubAccId} do not exist");

            return new GetCustomerStatusResponse();
        }

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

    public override async Task<GetBeLongingInfoResponse> GetBelongingInfo(GetBeLongingInfoRequest request, ServerCallContext context)
    {
        var isBelonging = await customerService.IsBelonging(request.DownlineId, request.ViewerId, request.SubAccountId, request.TransId);
        return new GetBeLongingInfoResponse
        {
            IsBelonging = isBelonging
        };
    }

    public override async Task<GetAllProductsResponse> GetAllProducts(GetAllProductsRequest request, ServerCallContext context)
    {
        var products = await customerService.GetAllProducts(request.SiteName, request.Currency);
        var result = new GetAllProductsResponse();

        result.Products.AddRange(products.Select(p => new ProductModel {
            Id = p.Id,
            Code = p.Code,
            Name = p.Name
        }));

        return result;
    }

    public override async Task<ClearProductsCacheResponse> ClearProductsCache(ClearProductsCacheRequest request, ServerCallContext context)
    {
        await customerService.ClearProductsCache(request.SiteName, request.Currency);
        return new ClearProductsCacheResponse { Result = true };
    }
}