
using System.Text;
using Alpha.BetLists.API.Criteria;
using Alpha.BetLists.API.Models;
using Fanex.Caching;
using Fanex.Data.Repository;
using Alpha.Utilities.Extensions;
using Alpha.Utilities.Constants;

namespace Alpha.BetLists.API.Services;

public interface IProductDetailService
{
    IEnumerable<ProductDetailModel> GetProductDetails(int viewerId, BetListsAppSettingsModel settings);
    string BuildProductDetailsXML(IEnumerable<int> validProductIds, IEnumerable<int> validResourceIds, IEnumerable<int> productOfferProductDetail);
}

public class ProductDetailService : IProductDetailService
{
    private const string ProductDetailsCacheKey = "Alpha.BetLists.API.Services.ProductDetailService.ProductDetails";
    private const int CustProductDetailsCacheInMinutes = 5;
    private const int ProductDetailsCacheInDay = 1;

    private readonly IDynamicRepository dynamicRepository;

    public ProductDetailService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public IEnumerable<ProductDetailModel> GetProductDetails(int viewerId, BetListsAppSettingsModel settings)
    {
        if (viewerId <= 0)
        {
            return Enumerable.Empty<ProductDetailModel>();
        }

        var productDetails = settings.ProductOfferProductDetail.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i));

        return ConfiguredCacheService.GetOrSet(
              $"{ProductDetailsCacheKey}-{viewerId}",
              () => GetProductDetails()
                  .Where(p => productDetails.Contains(p.ProductId)),
              new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(CustProductDetailsCacheInMinutes)));
    }

    public string BuildProductDetailsXML(
        IEnumerable<int> validProductIds,
        IEnumerable<int> validResourceIds,
        IEnumerable<int> productOfferProductDetail)
    {
        var existProducts = validProductIds.ToExistingDictionary();
        var existResources = validResourceIds.ToExistingDictionary();
        var validProductDetails = new List<ProductDetailModel>();
        var productids = new List<int>
        {
            ProductIdConstants.Allbet,
            ProductIdConstants.SexyGaming,
            ProductIdConstants.Pragmatic,
            ProductIdConstants.SpadeGaming,
            ProductIdConstants.MgRng
        };

        GetProductDetails()
           .Where(p => existProducts.ContainsKey(p.ProductId))
           .GroupBy(p => p.ProductId)
           .ToList()
           .ForEach(group =>
           {
               if (productids.Any(i => i == group.Key))
               {
                   validProductDetails.AddRange(group.Select(i => i));
               }

               if (IsSupportProductDetail(group.Key, productOfferProductDetail))
               {
                   validProductDetails.AddRange(group.Where(p => existResources.ContainsKey(p.ResourceId)));
               }
           });

        return validProductDetails.Any()
            ? BuildProductDetailsXML(validProductDetails)
            : string.Empty;
    }

    private static bool IsSupportProductDetail(int productId, IEnumerable<int> productOfferProductDetail)
        => productOfferProductDetail.Any(i => i == productId);

    private IEnumerable<ProductDetailModel> GetProductDetails()
        => ConfiguredCacheService.GetOrSet(
            ProductDetailsCacheKey,
            () => dynamicRepository.Fetch<ProductDetailModel>(new GetProductsDetailCriteria()),
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(ProductDetailsCacheInDay)));

    private static string BuildProductDetailsXML(IEnumerable<ProductDetailModel> productDetails)
    {
        var xml = new StringBuilder("<ROOT>");
        foreach (var product in productDetails)
        {
            foreach (var leagueId in product.LeagueGroupIds)
            {
                foreach (var typeId in product.TicketTypeIds)
                {
                    xml.AppendFormat(
                        "<ProductDetail ProductId=\"{0}\" Sporttype=\"{1}\" Bettype=\"{2}\" LeagueGroupID=\"{3}\" TicketType=\"{4}\"/>",
                        product.ProductId,
                        product.SportType,
                        product.BetType,
                        leagueId,
                        typeId);
                }
            }
        }

        xml.Append("</ROOT>");

        return xml.ToString();
    }
}