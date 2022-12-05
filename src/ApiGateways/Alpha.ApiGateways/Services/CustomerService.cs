using Alpha.ApiGateways.Models;
using GrpcCustomer;

namespace Alpha.ApiGateways.Services;

public interface ICustomerService
{
    Task<CustomerStatus> GetCustomerStatus(int custId, int subAccId = 0);

    Task<CustomerInfoModel> GetCustomerInfo(int custId, string downlineUsername);
}

public class CustomerService : ICustomerService
{
    private readonly Customer.CustomerClient customerClient;
    private readonly ILogger<Customer.CustomerClient> logger;

    public CustomerService(Customer.CustomerClient customerClient, ILogger<Customer.CustomerClient> logger)
    {
        this.customerClient = customerClient;
        this.logger = logger;
    }

    public async Task<CustomerStatus> GetCustomerStatus(int custId, int subAccId = 0)
    {
        logger.LogDebug("grpc client created, CustId = {@custId} SubAccId = {@subAccId}", custId, subAccId);
        var response = await customerClient.GetCustomerStatusAsync(new GetCustomerStatusRequest { CustId = custId, SubAccId = subAccId });
        logger.LogDebug("grpc response {@response}", response);

        return new CustomerStatus(
            response.CustId,
            response.SubAccId,
            response.IsActive,
            response.IsProtectedMode,
            response.IsLocked,
            response.IsSubAcc,
            response.HasReportPermission,
            response.HasMemberInformationPermission);
    }

    public async Task<CustomerInfoModel> GetCustomerInfo(int custId, string username)
    {
        var response = await customerClient.GetCustomerInfoAsync(
            new GetCustomerInfoRequest { CustId = custId, Username = username });

        return new CustomerInfoModel(response.CustId, (byte)response.RoleId, response.Site, response.Currency);
    }
}