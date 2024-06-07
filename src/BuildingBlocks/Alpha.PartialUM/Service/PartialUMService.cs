using Alpha.PartialUM.Extensions;
using Alpha.PartialUM.Helpers;
using Alpha.PartialUM.Models;
using Fanex.Caching;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Alpha.PartialUM.Services;

public interface IPartialUMService
{
    void ThrowIfUM(string settingKey, string siteName);
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

    public void ThrowIfUM(string settingKey, string siteName)
    {
        var functionNames = PartialUMSettingsHelper.GetFunctionNames(settingKey, partialUMSettings);
        if (!functionNames.Any())
        {
            return;
        }

        var umInfoDictionary = GetUMInfoDictionary(siteName);
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

    private IDictionary<string, UMInfoModel> GetUMInfoDictionary(string siteName)
        => ConfiguredCacheService
            .GetOrSet(
                $"Alpha.PartialUM.Services.MinimizedUM.{siteName}",
                () => GetUMInfoDictionaryFromDB(siteName),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(partialUMSettings.PartialUMCacheInSeconds)));

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