using Alpha.ApiKey.API.Criteria;
using Alpha.ApiKey.API.Models;
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
        var rawUsername = AesService.Decrypt(username);
        var rawPassword = AesService.Decrypt(password);
        var hashPassword = PasswordHashExtensions.Hash(rawPassword);

        var criteria = new VerifyExtApiKeyCriteria(rawKey, rawUsername, hashPassword);
        await dynamicRepository.ExecuteAsync(criteria);

        result = new VerifyExtApiKeyResult
        {
            IsAuthenticated = criteria.IsAuthenticated,
            CustId = criteria.CustId,
            SubAccId = criteria.SubAccId,
            Site = criteria.Site,
            Currency = criteria.Currency,
            ErrMsg = criteria.ErrMsg
        };

        if (result.IsAuthenticated)
        {
            var cachedTime = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromHours(1));
            await cacheService.SetAsync(cachedKey, result, cachedTime);
        }

        return result;
    }
}
