using System.Text;
using Alpha.ApiKey.API.Models;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Alpha.Utilities.Constants;
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
    (IEnumerable<int>, string) BuildProductDetailsXML(IEnumerable<int> validProductIds, string productDetailNames, HashSet<int> productOfferProductDetail, string language);
}

public class ProductDetailService : IProductDetailService
{
    private const string ProductDetailsCacheKey =
        "Alpha.Reports.API.Services.ProductDetailService.ProductDetails";
    private const int ProductDetailsCacheInDay = 1;

    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheRefService cacheRefService;

    public ProductDetailService(IDynamicRepository dynamicRepository, ICacheRefService cacheRefService)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheRefService = cacheRefService;
    }

    public IEnumerable<ProductDetailModel> GetProductDetails(ReportsAppSettingsModel settings)
    {
        var canViewProductDetails = CanViewProductDetails(settings);

        return GetProductDetails().Where(p => canViewProductDetails.ContainsKey(p.ProductId));
    }

    private IDictionary<int, bool> CanViewProductDetails(ReportsAppSettingsModel settings)
    {
        var canViewProductDetails = new Dictionary<int, bool>();

        var ps = settings.ProductOfferProductDetail.ToListInt();

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

    public (IEnumerable<int>, string) BuildProductDetailsXML(IEnumerable<int> validProductIds, string productDetailNames, HashSet<int> productOfferProductDetail, string language)
    {
        var userSelectedProductDetailNames = Enumerable.Empty<string>();
        var isSelectAllProductDetails = productDetailNames.Equals(ProductGroupConstants.All, StringComparison.OrdinalIgnoreCase);
        if (!isSelectAllProductDetails)
        {
            userSelectedProductDetailNames = productDetailNames.ToListString();
        }

        var validProductDetails = GetProductDetails()
            .Where(p => validProductIds.Contains(p.ProductId)
                && productOfferProductDetail.Contains(p.ProductId)
                && (isSelectAllProductDetails || userSelectedProductDetailNames.Contains(cacheRefService.GetProductDetailName(p.ResourceId, language), StringComparer.OrdinalIgnoreCase)));

        var validProductDetailIds = new List<int>();
        var xml = new StringBuilder();
        foreach (var product in validProductDetails)
        {
            validProductDetailIds.Add(product.ResourceId);
            xml.Append(product.ToString());
        }

        return (validProductDetailIds, validProductDetailIds.Any() ? $"<ROOT>{xml.ToString()}</ROOT>" : string.Empty);
    }
}
