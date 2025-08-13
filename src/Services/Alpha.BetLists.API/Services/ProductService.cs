using Alpha.BetLists.API.Models;
using Fanex.Data.Repository;
using Fanex.Caching;
using Alpha.BetLists.API.Criteria;
using Alpha.Utilities.Constants;

namespace Alpha.BetLists.API.Services;

public interface IProductService
{
    IEnumerable<int> GetAllProductIds(string group);
}

public class ProductService(IDynamicRepository dynamicRepository, ICacheService cacheService) : IProductService
{
    private const string ProductsCacheKey = "Alpha.BetLists.Services.ProductService.Products";
    private readonly IDynamicRepository dynamicRepository = dynamicRepository;
    private readonly ICacheService cacheService = cacheService;

    public IEnumerable<int> GetAllProductIds(string group)
    {
        var products = GetAllProducts();

        return FilterProductByGroup(products, group).Select(p => p.Id);
    }

    private IEnumerable<ProductModel> GetAllProducts()
    {
        var products = cacheService
            .GetOrSet(
            ProductsCacheKey,
            () => dynamicRepository.Fetch<ProductModel>(new GetProductsCriteria()),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

        return products;
    }

    private IEnumerable<ProductModel> FilterProductByGroup(IEnumerable<ProductModel> products, string group)
    {
        if (string.Equals(group, ProductGroupConstants.All, System.StringComparison.OrdinalIgnoreCase))
        {
            return products;
        }

        if (string.Equals(group, ProductGroupConstants.Sportsbook, System.StringComparison.OrdinalIgnoreCase))
        {
            return products.Where(p => p.Id == ProductIdConstants.Sportsbook);
        }

        return products.Where(p => p.Id != ProductIdConstants.Sportsbook);
    }
}