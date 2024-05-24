using System.Text;
using Alpha.ApiKey.API.Models;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Utilities.Extensions;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Services;

public interface IProductDetailService
{
    IEnumerable<ProductDetailModel> GetProductDetails(ReportsAppSettingsModel settings);
    string BuildProductDetailsXML(
        IEnumerable<int> validProductIds,
        IEnumerable<int> validResourceIds,
        IEnumerable<int> productOfferProductDetail
    );
}

public class ProductDetailService : IProductDetailService
{
    private const string ProductDetailsCacheKey =
        "Alpha.Reports.API.Services.ProductDetailService.ProductDetails";
    private const int ProductDetailsCacheInDay = 1;

    private readonly IDynamicRepository dynamicRepository;

    public ProductDetailService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public IEnumerable<ProductDetailModel> GetProductDetails(ReportsAppSettingsModel settings)
    {
        var canViewProductDetails = CanViewProductDetails(settings);

        return GetProductDetails().Where(p => canViewProductDetails.ContainsKey(p.ProductId));
    }

    private IDictionary<int, bool> CanViewProductDetails(ReportsAppSettingsModel settings)
    {
        var canViewProductDetails = new Dictionary<int, bool>();

        var ps = settings
            .ProductOfferProductDetail.Split(
                new[] { ",", ";" },
                StringSplitOptions.RemoveEmptyEntries
            )
            .Select(i => int.Parse(i));

        foreach (var product in ps)
        {
            canViewProductDetails[product] = true;
        }

        return canViewProductDetails;
    }

    public string BuildProductDetailsXML(
        IEnumerable<int> validProductIds,
        IEnumerable<int> validResourceIds,
        IEnumerable<int> productOfferProductDetail
    )
    {
        var existProducts = validProductIds.ToExistingDictionary();
        var existResources = validResourceIds.ToExistingDictionary();
        var validProductDetails = new List<ProductDetailModel>();

        GetProductDetails()
            .Where(p => existProducts.ContainsKey(p.ProductId))
            .GroupBy(p => p.ProductId)
            .ToList()
            .ForEach(group =>
            {
                if (IsSupportProductDetail(group.Key, productOfferProductDetail))
                {
                    validProductDetails.AddRange(
                        group.Where(p => existResources.ContainsKey(p.ResourceId))
                    );
                }
            });

        return validProductDetails.Any()
            ? BuildProductDetailsXML(validProductDetails)
            : string.Empty;
    }

    private static bool IsSupportProductDetail(
        int productId,
        IEnumerable<int> productOfferProductDetail
    ) => productOfferProductDetail.Any(i => i == productId);

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
