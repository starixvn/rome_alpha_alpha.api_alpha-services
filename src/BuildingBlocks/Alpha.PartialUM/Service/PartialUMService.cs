using Alpha.PartialUM.Extensions;
using Alpha.PartialUM.Helpers;
using Alpha.PartialUM.Models;
using Fanex.Caching;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Alpha.PartialUM.Services;

public interface IPartialUMService
{
    void ThrowIfUM(string settingKey);
}

public class PartialUMService : IPartialUMService
{
    private readonly PartialUMSettings partialUMSettings;
    private readonly IPartialUMDataAccess partialUMDataAccess;

    public PartialUMService(
        IOptions<PartialUMSettings> partialUMSettings,
        IPartialUMDataAccess partialUMDataAccess)
    {
        this.partialUMSettings = partialUMSettings.Value;
        this.partialUMDataAccess = partialUMDataAccess;
    }

    public void ThrowIfUM(string settingKey)
    {
        var functionNames = PartialUMSettingsHelper.GetFunctionNames(settingKey, partialUMSettings);
        if (!functionNames.Any())
        {
            return;
        }

        var umInfoDictionary = GetUMInfoDictionary();
        if (umInfoDictionary.Count == 0)
        {
            return;
        }

        foreach (var function in functionNames)
        {
            if (umInfoDictionary.TryGetValue(function, out var umInfo))
            {
                var info = new PartialUMExceptionModel(umInfo.BeginTime.ToLongString(), umInfo.EndTime.ToLongString());

                throw new RpcException(new Status(StatusCode.Unavailable, info.Message));
            }
        }
    }

    private IDictionary<string, UMInfoModel> GetUMInfoDictionary()
        => ConfiguredCacheService
            .GetOrSet(
                $"Alpha.PartialUM.Services.MinimizedUM.{partialUMSettings.SiteGroup}",
                () => GetUMInfoDictionaryFromDB(partialUMSettings.SiteGroup),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(partialUMSettings.BetListsCacheInSeconds)));

    private IDictionary<string, UMInfoModel> GetUMInfoDictionaryFromDB(string siteGroup)
    {
        var umInfoList = partialUMDataAccess.GetUMInfoList(siteGroup);
        var umInfoDictionary = new Dictionary<string, UMInfoModel>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in umInfoList)
        {
            umInfoDictionary[item.FunctionTypeName] = item;
        }

        return umInfoDictionary;
    }
}