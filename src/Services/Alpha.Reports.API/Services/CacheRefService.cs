using Fanex.RemotingRef;

namespace Alpha.Reports.API.Services;

public interface ICacheRefService
{
    string GetProductName(int productId, string language);

    string GetProductDetailName(int productDetailId, string language);
}

public class CacheRefService : ICacheRefService
{
    private readonly IRemotingRefService remotingRefService;

    public CacheRefService(IRemotingRefService remotingRefService)
    {
        this.remotingRefService = remotingRefService;
    }

    public string GetProductName(int productId, string language)
        => GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), language);

    public string GetProductDetailName(int productDetailId, string language)
        => GetResourceDataByRefIdAndResourceId("ProductDetail", productDetailId.ToString(), language);

    private string GetResourceDataByRefIdAndResourceId(string refId, string resourceId, string language)
        => remotingRefService.GetResourceDataByRefIdAndResourceId(
        refId,
        resourceId,
        language,
        string.Empty);
}

