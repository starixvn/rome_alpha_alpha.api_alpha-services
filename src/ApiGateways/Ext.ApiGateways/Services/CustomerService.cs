using Ext.ApiGateways.Models;
using GrpcCustomer;

namespace Ext.ApiGateways.Services;

public interface ICustomerService
{
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

    public async Task<CustomerInfoModel> GetCustomerInfo(int custId, string username)
    {
        var response = await customerClient.GetCustomerInfoAsync(
            new GetCustomerInfoRequest { CustId = custId, Username = username });

        return new CustomerInfoModel(response.CustId, (byte)response.RoleId, response.Site, response.Currency);
    }
}