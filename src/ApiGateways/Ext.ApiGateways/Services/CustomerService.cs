using Ext.ApiGateways.Models;
using GrpcCustomer;

namespace Ext.ApiGateways.Services;

public interface ICustomerService
{
    Task<CustomerInfoModel> GetCustomerInfo(int custId, string downlineUsername);
    Task<bool> GetBelongingInfo(int custId, int downlineId, int subAccountId, long transId);
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

    public async Task<CustomerInfoModel> GetCustomerInfo(int custId, string downlineUsername)
    {
        var response = await customerClient.GetCustomerInfoAsync(
            new GetCustomerInfoRequest { CustId = custId, Username = downlineUsername });

        return new CustomerInfoModel(response.CustId, (byte)response.RoleId, response.Site, response.Currency);
    }

    public async Task<bool> GetBelongingInfo(int custId, int downlineId, int subAccountId, long transId)
    {
        var response = await customerClient.GetBelongingInfoAsync(
            new GetBeLongingInfoRequest
            {
                ViewerId = custId,
                DownlineId = downlineId,
                SubAccountId = subAccountId,
                TransId = transId
            });

        return response.IsBelonging;
    }
}