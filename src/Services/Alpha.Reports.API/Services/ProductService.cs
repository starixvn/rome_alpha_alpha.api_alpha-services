using Alpha.Reports.API.Constants;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Utilities.Constants;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Services;

public interface IProductService
{
    IEnumerable<int> FilterProductIdsByCustomer(
        string language,
        IEnumerable<int> productIds,
        DateTime? minReportDate = null);

    IEnumerable<ProductModel> GetProductsForCustomer(string language, DateTime? minReportDate = null);

    IEnumerable<int> FilterProductIdsByGroup(IEnumerable<int> products, string group);
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

    public IEnumerable<int> FilterProductIdsByCustomer(
        string language,
        IEnumerable<int> productIds,
        DateTime? minReportDate = null)
    {
        var customerProductIds = GetProductsForCustomer(language, minReportDate).Select(product => product.Id);

        var productIdList = productIds == null || !productIds.Any()
            ? customerProductIds
            : customerProductIds.Where(product => productIds.Contains(product));

        return productIdList;
    }

    public IEnumerable<ProductModel> GetProductsForCustomer(string language, DateTime? minReportDate = null)
    {
        var custProducts = GetProductsForReport(language);

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

    private IEnumerable<ProductModel> GetProductsForReport(string language)
    {
        var products = cacheService
            .GetOrSet(
            ProductsCacheKey,
            () => dynamicRepository.Fetch<ProductModel>(new GetProductsCriteria())
                .Select(product =>
                {
                    var p = new ProductModel(product.Id, product.Name, product.EndOfSupportDate, product.Type, product.OrderNo);
                    p.UpdateProductName(GetProductName(product.Id, language));
                    return p;
                }),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return products;
    }

    public string GetProductName(int productId, string language)
        => cacheRefService.GetProductName(productId, language);

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
}