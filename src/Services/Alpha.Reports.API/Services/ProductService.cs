using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Utilities.Constants;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Services;

public interface IProductService
{
    IEnumerable<int> FilterProductIdsByCustomer(IEnumerable<int> productIds, DateTimeOffset? minReportDate = null);

    IEnumerable<int> FilterProductIdsByGroup(IEnumerable<int> products, string group);
}

public class ProductService : IProductService
{
    private const string ProductsCacheKey = "Alpha.Reports.Services.Impl.ProductServiceImpl.Products";
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;

    public ProductService(IDynamicRepository dynamicRepository, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
    }

    public IEnumerable<int> FilterProductIdsByCustomer(IEnumerable<int> productIds, DateTimeOffset? minReportDate = null)
    {
        var customerProductIds = GetProductsForCustomer(minReportDate).Select(product => product.Id);

        var productIdList = productIds == null || !productIds.Any()
            ? customerProductIds
            : customerProductIds.Where(product => productIds.Contains(product));

        return productIdList;
    }

    private IEnumerable<ProductModel> GetProductsForCustomer(DateTimeOffset? minReportDate = null)
    {
        var custProducts = GetProductsForReport();

        if (custProducts == null)
        {
            return new List<ProductModel> { };
        }

        return custProducts.Where(product => IsRemainSupportProduct(product, minReportDate)).ToList();
    }

    private static bool IsRemainSupportProduct(ProductModel product, DateTimeOffset? minReportDate)
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
        if (products == null)
        {
            return Enumerable.Empty<int>();
        }

        if (IsAllGroup(group))
        {
            return products;
        }

        if (IsSportsbookGroup(group))
        {
            return FilterSportsbookProducts(products);
        }

        return FilterNonSportsbookProducts(products);
    }

    private static bool IsAllGroup(string group)
    {
        return string.IsNullOrWhiteSpace(group) || 
               string.Equals(group, ProductGroupConstants.All, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSportsbookGroup(string group)
    {
        return string.Equals(group, ProductGroupConstants.Sportsbook, StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<int> FilterSportsbookProducts(IEnumerable<int> products)
    {
        return products.Where(p => p == ProductIdConstants.Sportsbook);
    }

    private static IEnumerable<int> FilterNonSportsbookProducts(IEnumerable<int> products)
    {
        var excludedProducts = new[] { ProductIdConstants.Sportsbook, ProductIdConstants.CashBilling };
        return products.Where(p => !excludedProducts.Contains(p));
    }
}