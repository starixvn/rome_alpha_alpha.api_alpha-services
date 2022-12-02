using Alpha.ApiKey.API.Models;
using Alpha.Reports.API.Constans;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Fanex.Caching;
using Fanex.Data.Repository;
using Microsoft.Extensions.Options;

namespace Alpha.Reports.API.Services;

public interface IProductService
{
    IEnumerable<int> FilterProductIdsByCustomer(
        int viewerId,
        string site,
        int currencyId,
        IEnumerable<int> productIds,
        DateTime? minReportDate = null,
        bool isEnableSabaPromotion = false);
}

public class ProductService : IProductService
{
    private const int CustProductsCacheTimeInMinutes = 5;
    private const string ProductsCacheKey = "Alpha.Reports.Services.Impl.ProductServiceImpl.Products";
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;
    private readonly ReportsAppSettingsModel settings;

    public ProductService(
        IDynamicRepository dynamicRepository,
        ICacheService cacheService,
        IOptions<ReportsAppSettingsModel> settings)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
        this.settings = settings.Value;
    }

    public IEnumerable<int> FilterProductIdsByCustomer(
        int viewerId,
        string site,
        int currencyId,
        IEnumerable<int> productIds,
        DateTime? minReportDate = null,
        bool isEnableSabaPromotion = false)
    {
        var customerProductIds = GetProductsForCustomer(viewerId, site, currencyId, minReportDate, isEnableSabaPromotion)
            .Select(product => product.Id);

        var productIdList = productIds == null || !productIds.Any()
            ? customerProductIds
            : customerProductIds.Where(product => productIds.Contains(product));

        return productIdList;
    }

    public IEnumerable<ProductModel> GetProductsForCustomer(
        int viewerId,
        string site,
        int currencyId,
        DateTime? minReportDate = null,
        bool isEnableSabaPromotion = false)
    {
        if (viewerId <= 0)
        {
            return Enumerable.Empty<ProductModel>();
        }

        var custProducts = cacheService
            .GetOrSet(
            $"Alpha.Reports.Services.Impl.ProductsForCustomer-{viewerId}",
            () => GetProductsForReport()
                .Where(product => CanViewBySite(viewerId, site, currencyId, product.Id))
                .Select(product => new ProductModel(product.Id, product.Name, product.EndOfSupportDate, product.Type, product.OrderNo, this.GetProductName)),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(CustProductsCacheTimeInMinutes)));

        if (custProducts != null)
        {
            custProducts = custProducts.Where(product => IsRemainSupportProduct(product, minReportDate));
        }

        if (isEnableSabaPromotion)
        {
            var endOfSupportDate = custProducts.Max(product => product.EndOfSupportDate);
            var sabaProduct = new ProductModel(ProductIdConstants.SabaPromotion, null, endOfSupportDate, 0, 0, this.GetProductName);
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

    public string GetProductName(int productId)
    {
        return "Name";
    }
}