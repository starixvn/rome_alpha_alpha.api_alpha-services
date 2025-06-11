using Alpha.ApiKey.API.Criteria;
using Alpha.ApiKey.API.Models;
using Alpha.Utilities.Constants;
using Alpha.Utilities.Extensions;
using Alpha.Utilities.Helpers;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.ApiKey.API.Services;

public interface IExtApiKeyService
{
    Task<VerifyExtApiKeyResult> VerifyApiKey(string key, string username, string password);
}

public class ExtApiKeyService : IExtApiKeyService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;

    public ExtApiKeyService(IDynamicRepository dynamicRepository, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
    }

    public async Task<VerifyExtApiKeyResult> VerifyApiKey(string key, string username, string password)
    {
        var cachedKey = $"{key}_{username}";

        var result = await cacheService.GetAsync<VerifyExtApiKeyResult>(cachedKey);
        if (result != null && result.IsAuthenticated)
        {
            return result;
        }

        var rawKey = AesService.Decrypt(key);
        var groupNames = await GetGroupNames(rawKey);

        if (string.IsNullOrWhiteSpace(groupNames))
        {
            return new VerifyExtApiKeyResult
            {
                IsAuthenticated = false,
                ErrMsg = LoggingConstants.WrongSecretKey,
                Site = string.Empty,
            };
        }

        var rawUsername = AesService.Decrypt(username);
        var rawPassword = AesService.Decrypt(password);
        var hashPassword = PasswordHashExtensions.Hash(rawPassword);
        var criteria = new VerifyExtApiKeyCriteria(groupNames, rawUsername, hashPassword);

        if (!criteria.IsValid())
        {
            criteria.ErrMsg = LoggingConstants.KeyUsernamePasswordIsInvalid;
        }

        var custs = await dynamicRepository.FetchAsync<CustInfoModel>(criteria);
        var isSuccess = criteria.ErrCode == 0 && custs.Any();

        if (!isSuccess)
        {
            return new VerifyExtApiKeyResult
            {
                IsAuthenticated = false,
                ErrMsg = criteria.ErrMsg,
                Site = string.Empty,
            };
        }

        var custInfo = custs.FirstOrDefault();
        result = new VerifyExtApiKeyResult
        {
            IsAuthenticated = true,
            CustId = custInfo.CustId,
            SubAccId = custInfo.SubAccId,
            ErrMsg = criteria.ErrMsg,
            Site = custInfo.Site,
            Currency = custInfo.Currency
        };
        await cacheService.SetAsync(cachedKey, result, new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));

        return result;
    }

    public async Task<string> GetGroupNames(string key)
    {
        var result = await cacheService.GetOrSetAsync(
            "All_SecretKey_Group_CachedKey",
            async () =>
            {
                var criteria = new GetAllSecretKeyGroupsCriteria();
                return await dynamicRepository.FetchAsync<SecretKeyModel>(criteria);
            },
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1)));

        if (result == null)
        {
            return string.Empty;
        }

        var names = result
            .Where(sg => string.Equals(sg.Key, key, StringComparison.OrdinalIgnoreCase))
            .Select(sg => sg.GroupName);

        return string.Join(',', names);
    }
}