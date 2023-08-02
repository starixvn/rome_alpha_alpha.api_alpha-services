using Alpha.ApiKey.API.Models;
using Alpha.Reports.API.Constants;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Utilities.Constans;
using Alpha.Utilities.Helpers;
using Alpha.Utilities.Models;
using Fanex.Caching;
using Fanex.Data.Repository;
using GrpcCustomer;
using Microsoft.Extensions.Options;

namespace Alpha.Reports.API.Services;

public interface IProductService
{
    Task<IEnumerable<int>> FilterProductIdsByCustomer(
        int viewerId,
        int roleId,
        string site,
        int currencyId,
        string language,
        IEnumerable<int> productIds,
        DateTime? minReportDate = null,
        bool isEnableSabaPromotion = false);

    Task<IEnumerable<ProductModel>> GetProductsForCustomer(
        int viewerId,
        int roleId,
        string site,
        int currencyId,
        string language,
        DateTime? minReportDate = null,
        bool isEnableSabaPromotion = false);

    bool CanViewBySite(int viewerId, string site, int currencyId, int productId);

    Task<IEnumerable<ProductStatusModel>> GetProductStatus(int viewerId, int roleId);

    bool CanViewProduct(int productId, string site, IEnumerable<ProductStatusModel> products, ReportsAppSettingsModel settings);

    IEnumerable<int> FilterProductIdsByGroup(IEnumerable<int> products, string group);
}

public class ProductService : IProductService
{
    private const int CustProductsCacheTimeInMinutes = 5;
    private const string ProductsCacheKey = "Alpha.Reports.Services.Impl.ProductServiceImpl.Products";
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;
    private readonly ReportsAppSettingsModel settings;
    private readonly Customer.CustomerClient customerClient;
    private readonly ICacheRefService cacheRefService;

    public ProductService(
        IDynamicRepository dynamicRepository,
        ICacheService cacheService,
        IOptions<ReportsAppSettingsModel> settings,
        Customer.CustomerClient customerClient,
        ICacheRefService cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
        this.settings = settings.Value;
        this.customerClient = customerClient;
        this.cacheRefService = cacheRefService;
    }

    public async Task<IEnumerable<int>> FilterProductIdsByCustomer(
        int viewerId,
        int roleId,
        string site,
        int currencyId,
        string language,
        IEnumerable<int> productIds,
        DateTime? minReportDate = null,
        bool isEnableSabaPromotion = false)
    {
        var customerProductIds =
            (await GetProductsForCustomer(viewerId, roleId, site, currencyId, language, minReportDate, isEnableSabaPromotion))
            .Select(product => product.Id);

        var productIdList = productIds == null || !productIds.Any()
            ? customerProductIds
            : customerProductIds.Where(product => productIds.Contains(product));

        return productIdList;
    }

    public async Task<IEnumerable<ProductModel>> GetProductsForCustomer(
        int viewerId,
        int roleId,
        string site,
        int currencyId,
        string language,
        DateTime? minReportDate = null,
        bool isEnableSabaPromotion = false)
    {
        if (viewerId <= 0)
        {
            return Enumerable.Empty<ProductModel>();
        }

        var productStatus = await GetProductStatus(viewerId, roleId);

        var custProducts = cacheService
            .GetOrSet(
            $"Alpha.Reports.Services.Impl.ProductsForCustomer-{viewerId}",
            () => GetProductsForReport()
                .Where(product => CanViewProduct(product.Id, site, productStatus, settings) || CanViewBySite(viewerId, site, currencyId, product.Id))
                .Select(product =>
                {
                    var p = new ProductModel(product.Id, product.Name, product.EndOfSupportDate, product.Type, product.OrderNo);
                    p.UpdateProductName(GetProductName(product.Id, language));
                    return p;
                }),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(CustProductsCacheTimeInMinutes)));

        if (custProducts != null)
        {
            custProducts = custProducts.Where(product => IsRemainSupportProduct(product, minReportDate));
        }

        if (isEnableSabaPromotion)
        {
            var endOfSupportDate = custProducts.Max(product => product.EndOfSupportDate);
            var sabaProduct = new ProductModel(ProductIdConstants.SabaPromotion, null, endOfSupportDate, 0, 0);
            sabaProduct.UpdateProductName(GetProductName(sabaProduct.Id, language));
            var sabaProducts = new List<ProductModel> { sabaProduct };
            custProducts = sabaProducts.Concat(custProducts);
        }

        return custProducts;
    }

    public bool CanViewBySite(int viewerId, string site, int currencyId, int productId)
    {
        var custProducts = cacheService
            .GetOrSet(
            $"Alpha.Reports.Services.Impl.ProductsBySite-{viewerId}",
            () => dynamicRepository.Fetch<ProductBySiteModel>(new GetOfferedProductsBySiteCriteria(viewerId, site, currencyId, settings.SettingStatus)),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(CustProductsCacheTimeInMinutes)));

        return custProducts.Any(i => i.ProductId == productId);
    }

    private static bool IsRemainSupportProduct(ProductModel product, DateTime? minReportDate)
        => !minReportDate.HasValue
        || !product.EndOfSupportDate.HasValue
        || minReportDate.Value <= product.EndOfSupportDate.Value;

    private IEnumerable<ProductModel> GetProductsForReport()
    {
        var products = cacheService
            .GetOrSet(
            ProductsCacheKey,
            () => dynamicRepository.Fetch<ProductModel>(new GetProductsCriteria()),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return products;
    }

    public bool CanViewProduct(int productId, string site, IEnumerable<ProductStatusModel> products, ReportsAppSettingsModel settings)
    {
        var result = UserPermissionHelper.HasPermissionOnProduct(productId, site, products, settings);
        return result;
    }

    public async Task<IEnumerable<ProductStatusModel>> GetProductStatus(int viewerId, int roleId)
    {
        var productStatus = (await customerClient.GetCustomerProductsAsync(new GetCustomerProductsRequest { CustId = viewerId, RoleId = roleId }))
            .Products.Select(p => new ProductStatusModel
            {
                Id = p.Id,
                IsSynced = p.IsSynced,
                IsHidden = p.IsHidden
            });

        return productStatus;
    }

    public string GetProductName(int productId, string language)
        => cacheRefService.GetProductName(productId, language);


    public IEnumerable<int> FilterProductIdsByGroup(IEnumerable<int> products, string group)
    {
        if (string.Equals(group, ProductGroupConstants.All, System.StringComparison.OrdinalIgnoreCase))
        {
            return products;
        }

        if (string.Equals(group, ProductGroupConstants.Sportsbook, System.StringComparison.OrdinalIgnoreCase))
        {
            return products.Where(p => p == ProductIdConstants.Sportsbook);
        }

        return products.Where(p => p != ProductIdConstants.Sportsbook);
    }
}