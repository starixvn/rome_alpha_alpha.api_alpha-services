using System.Text;
using Alpha.Customer.API.Configurations;
using Alpha.Customer.API.Criteria;
using Alpha.Customer.API.DataAccesses;
using Alpha.Customer.API.Models;
using Alpha.Customer.API.Params;
using Fanex.Caching;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Alpha.Customer.API.Services;

public interface ICustomerService
{
    Task<CustomerStatus> GetCustomerStatus(CustomerStatusParam customer);

    Task<IEnumerable<CustomerStatus>> GetCustomerStatus(IEnumerable<CustomerStatusParam> customers);

    Task<DownlineInfo> GetDownlineInfo(int custId, string downlineUsername);

    Task<CustomerInfoModel> GetCustomerInfo(int custId, string username);
}

public class CustomerService : ICustomerService
{
    private const string CustomerStatusCacheKey = "Alpha.Customer.API.CustomerStatus";

    private readonly ICacheService cacheService;
    private readonly CustomerAppSettings appSettings;
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICustomerDataAccess customerDataAccess;

    public CustomerService(
        ICacheService cacheService,
        IOptions<CustomerAppSettings> options,
        IDynamicRepository dynamicRepository,
        ICustomerDataAccess customerDataAccess)
    {
        this.cacheService = cacheService;
        appSettings = options.Value;
        this.dynamicRepository = dynamicRepository;
        this.customerDataAccess = customerDataAccess;
    }

    public Task<CustomerStatus> GetCustomerStatus(CustomerStatusParam customer)
        => cacheService.GetOrSetAsync(
            $"{CustomerStatusCacheKey}.{customer.CustId}.{customer.SubAccId}",
            async () => (await GetCustomerStatus(new[] { customer })).FirstOrDefault(),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(appSettings.CustomerStatusCacheInSeconds)));

    public Task<IEnumerable<CustomerStatus>> GetCustomerStatus(IEnumerable<CustomerStatusParam> customers)
        => dynamicRepository.FetchAsync<CustomerStatus>(new GetCustomerStatusCriteria(BuildCustListParam(customers)));

    private static string BuildCustListParam(IEnumerable<CustomerStatusParam> customers)
    {
        var spParams = new StringBuilder();
        spParams.Append("<Root>");

        foreach (var item in customers)
        {
            spParams.Append(item.ToXml());
        }

        spParams.Append("</Root>");
        return spParams.ToString();
    }

    public async Task<DownlineInfo> GetDownlineInfo(int custId, string downlineUsername)
    {
        var command = new GetDownlineInfoCommand(custId, 0, downlineUsername);
        await dynamicRepository.ExecuteAsync(command);

        return new DownlineInfo(command.CustId, (byte)command.RoleId, command.IsSubAcc);
    }

    public async Task<CustomerInfoModel> GetCustomerInfo(int custId, string username)
    {
        var criteria = new GetCustomerInfoCriteria(custId, username);

        return await customerDataAccess.GetCustomerInfo(criteria);
    }
}
