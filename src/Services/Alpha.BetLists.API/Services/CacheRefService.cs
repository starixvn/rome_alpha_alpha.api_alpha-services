using Fanex.RemotingRef;

namespace Alpha.BetLists.API.Services;

public interface ICacheRefService
{
    string GetProductName(int productId, string language);

    string GetResourceDataByRefIdAndResourceId(string refId, int resourceId, string language);
}

public class CacheRefService : ICacheRefService
{
    private readonly IRemotingRefService remotingRefService;

    public CacheRefService(IRemotingRefService remotingRefService)
    {
        this.remotingRefService = remotingRefService;
    }

    public string GetProductName(int productId, string language)
        => GetResourceDataByRefIdAndResourceId("Product", productId, language);

    public string GetResourceDataByRefIdAndResourceId(string refId, int resourceId, string language)
        => remotingRefService.GetResourceDataByRefIdAndResourceId(
            refId,
            resourceId.ToString(),
            language,
            string.Empty);
}