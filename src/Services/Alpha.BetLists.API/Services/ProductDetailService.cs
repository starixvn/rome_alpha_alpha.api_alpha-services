using System.Text;
using Alpha.BetLists.API.Criteria;
using Alpha.BetLists.API.Models;
using Alpha.Utilities.Extensions;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Services;

public interface IProductDetailService
{
    string BuildProductDetailsXML(IEnumerable<int> validProductIds);
}

public class ProductDetailService : IProductDetailService
{
    private const string ProductDetailsCacheKey =
        "Alpha.BetLists.API.Services.ProductDetailService.ProductDetails";

    private const int ProductDetailsCacheInDay = 1;

    private readonly IDynamicRepository dynamicRepository;

    public ProductDetailService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }    

    public string BuildProductDetailsXML(IEnumerable<int> validProductIds)
    {
        var existProducts = validProductIds.ToExistingDictionary();
        var validProductDetails = new List<ProductDetailModel>();

        GetProductDetails()
            .Where(p => existProducts.ContainsKey(p.ProductId))
            .GroupBy(p => p.ProductId)
            .ToList()
            .ForEach(group =>
            {
                validProductDetails.AddRange(group.Select(i => i));
            });

        return validProductDetails.Any()
            ? BuildProductDetailsXML(validProductDetails)
            : string.Empty;
    }
  
    private IEnumerable<ProductDetailModel> GetProductDetails() =>
        ConfiguredCacheService.GetOrSet(
            ProductDetailsCacheKey,
            () => dynamicRepository.Fetch<ProductDetailModel>(new GetProductsDetailCriteria()),
            new CacheItemOptions().SetAbsoluteExpiration(
                TimeSpan.FromDays(ProductDetailsCacheInDay)
            )
        );

    private static string BuildProductDetailsXML(IEnumerable<ProductDetailModel> productDetails)
    {
        var xml = new StringBuilder("<ROOT>");
        foreach (var product in productDetails)
        {
            xml.Append(product.ToString());
        }

        xml.Append("</ROOT>");

        return xml.ToString();
    }
}
