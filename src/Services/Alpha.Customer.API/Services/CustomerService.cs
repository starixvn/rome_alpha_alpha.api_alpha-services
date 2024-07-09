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

    Task<DownlineInfo> GetDownlineInfo(int custId, string downlineUsername);

    Task<CustomerInfoModel> GetCustomerInfo(int custId, string username);

    Task<bool> IsBelonging(int custId, int sessionCustId,int subAccountId, long transId);

    Task<IEnumerable<ProductModel>> GetAllProducts(string siteName, int currency);
    Task ClearProductsCache(string siteName, int currency);
}

public class CustomerService : ICustomerService
{
    private const string CustomerStatusCacheKey = "Alpha.Customer.API.CustomerStatus";
    private const string DownlineInfoCacheKey = "Alpha.Customer.API.DownlineInfo";
    private const string CustomerInfoCacheKey = "Alpha.Customer.API.CustomerInfo";
    private const string ProductsCacheKey = "Alpha.Customer.API.Products";

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
            async () =>
            {
                var criteria = new GetCustomerStatusCriteria(new[] { customer });
                var status = await dynamicRepository.FetchAsync<CustomerStatus>(criteria);

                return status.FirstOrDefault();
            },
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(appSettings.CustomerStatusCacheInSeconds)));


    public async Task<DownlineInfo> GetDownlineInfo(int custId, string downlineUsername)
        => await cacheService.GetOrSetAsync(
            $"{DownlineInfoCacheKey}.{custId}.{downlineUsername}",
            async () =>
            {
                var command = new GetDownlineInfoCommand(custId, 0, downlineUsername);
                await dynamicRepository.ExecuteAsync(command);

                return new DownlineInfo(command.CustId, (byte)command.RoleId, command.IsSubAcc);
            },
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(appSettings.CustomerStatusCacheInSeconds)));

    public Task<CustomerInfoModel> GetCustomerInfo(int custId, string username)
        => cacheService.GetOrSetAsync(
            $"{CustomerInfoCacheKey}.{custId}.{username}",
            async () => await customerDataAccess.GetCustomerInfo(new GetCustomerInfoCriteria(custId, username)),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(appSettings.CustomerStatusCacheInSeconds)));

    public async Task<bool> IsBelonging(int custId, int sessionCustId, int subAccountId, long transId)
    {
        var command = new GetBelongingInfoCommand(custId, sessionCustId, subAccountId, transId);
        await dynamicRepository.ExecuteAsync(command);
        return command.RoleId > 0 && command.IsActive;
    }

    public Task<IEnumerable<ProductModel>> GetAllProducts(string siteName, int currency)
        => cacheService.GetOrSetAsync(
            $"{ProductsCacheKey}_{siteName}_{currency}",
            async () =>
            {
                var criteria = new GetAllProductCriteria(siteName, currency);
                var products = await dynamicRepository.FetchAsync<ProductModel>(criteria);
                return products;
            },
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(appSettings.ProductCacheInSeconds)));

    public Task ClearProductsCache(string siteName, int currency) 
        => cacheService.RemoveAsync($"{ProductsCacheKey}_{siteName}_{currency}");
}
