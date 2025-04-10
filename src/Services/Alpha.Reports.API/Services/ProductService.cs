using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Utilities.Constants;
using Alpha.Utilities.Extensions;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Services;

public interface IProductService
{
    IEnumerable<int> FilterProductIdsByCustomer(IEnumerable<int> productIds, DateTime? minReportDate = null);

    IEnumerable<int> FilterProductIdsByGroup(IEnumerable<int> products, string group);

    IEnumerable<int> FilterProductIdsByCustomer(string productNames, string language, DateTime? minReportDate = null);

    Task<IEnumerable<ProductModel>> GetProducts(string site, int currencyId, byte settingStatus, DateTime? minReportDate);
}

public class ProductService : IProductService
{
    private const string ProductsCacheKey = "Alpha.Reports.Services.Impl.ProductServiceImpl.Products";
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;
    private readonly ICacheRefService cacheRefService;

    public ProductService(IDynamicRepository dynamicRepository, ICacheService cacheService, ICacheRefService cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
        this.cacheRefService = cacheRefService;
    }

    public IEnumerable<int> FilterProductIdsByCustomer(IEnumerable<int> productIds, DateTime? minReportDate = null)
    {
        var customerProductIds = GetProductsForCustomer(minReportDate).Select(product => product.Id);

        var productIdList = productIds == null || !productIds.Any()
            ? customerProductIds
            : customerProductIds.Where(product => productIds.Contains(product));

        return productIdList;
    }

    private IEnumerable<ProductModel> GetProductsForCustomer(DateTime? minReportDate = null)
    {
        var custProducts = GetProductsForReport();

        if (custProducts == null)
        {
            return new List<ProductModel> { };
        }

        return custProducts.Where(product => IsRemainSupportProduct(product, minReportDate)).ToList();
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
            () => dynamicRepository.Fetch<ProductModel>(new GetProductsCriteria())
                .Select(product => new ProductModel(product.Id, product.Name, product.EndOfSupportDate, product.Type, product.OrderNo)),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return products;
    }

    public IEnumerable<int> FilterProductIdsByGroup(IEnumerable<int> products, string group)
    {
        if (string.Equals(group, ProductGroupConstants.All, StringComparison.OrdinalIgnoreCase))
        {
            return products;
        }

        if (string.Equals(group, ProductGroupConstants.Sportsbook, StringComparison.OrdinalIgnoreCase))
        {
            return products.Where(p => p == ProductIdConstants.Sportsbook);
        }

        return products.Where(p => p != ProductIdConstants.Sportsbook);
    }

    public IEnumerable<int> FilterProductIdsByCustomer(string productNames, string language, DateTime? minReportDate = null)
    {
        var products = GetProductsForCustomer(minReportDate);

        if (!productNames.Equals(ProductGroupConstants.All, StringComparison.OrdinalIgnoreCase))
        {
            var productNamesList = productNames.ToListString();
            products = products.Where(p => productNamesList.Contains(cacheRefService.GetProductName(p.Id, language), StringComparer.OrdinalIgnoreCase)
                || productNamesList.Contains(cacheRefService.GetProductName(p.Type, language), StringComparer.OrdinalIgnoreCase));
        }

        return products.Select(p => p.Id);
    }

    public async Task<IEnumerable<ProductModel>> GetProducts(string site, int currencyId, byte settingStatus, DateTime? minReportDate)
    {
        var products = await cacheService.GetOrSetAsync(
            $"{ProductsCacheKey}-{site}-{currencyId}",
            async () =>
            {
                var products = await dynamicRepository.FetchAsync<ProductModel>(new GetProductsBySiteCurrencyCriteria(site, currencyId, settingStatus));
                return products.Where(p => !p.IsHidden);
            },
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return products.Where(p => IsRemainSupportProduct(p, minReportDate));
    }
}