using Ext.ApiGateways.Criteria;
using Ext.ApiGateways.Models;
using Fanex.Data.Repository;
using Ext.ApiGateways.Extensions;
using Fanex.Caching;
using Alpha.Utilities.Constants;
using Alpha.Utilities.Helpers;

namespace Ext.ApiGateways.Services
{
    public interface IApiKeyService
    {
        Task<VerifyApiKeyResult> VerifyApiKey(string key, string username, string password);
    }

    public class ApiKeyService : IApiKeyService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly ICacheService cacheService;
        private readonly TimeSpan cacheTime = TimeSpan.FromHours(1);

        public ApiKeyService(
            IDynamicRepository dynamicRepository,
            ICacheService cacheService)
        {
            this.dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
        }

        public async Task<VerifyApiKeyResult> VerifyApiKey(string key, string username, string password)
        {
            var cachedKey = $"{key}_{username}";
            var result = await cacheService.GetAsync<VerifyApiKeyResult>(cachedKey);
            if (result == null)
            {
                var rawKey = AesService.Decrypt(key);
                var rawUsername = AesService.Decrypt(username);
                var rawPassword = AesService.Decrypt(password);
                var hashPassword = PasswordHashExtensions.Hash(rawPassword);

                var criteria = new VerifyApiKeyCriteria(rawKey, rawUsername, hashPassword);

                if (!criteria.IsValid())
                {
                    criteria.ErrMsg = LoggingConstants.KeyUsernamePasswordIsInvalid;
                }

                await dynamicRepository.ExecuteAsync(criteria);

                result = new VerifyApiKeyResult
                {
                    IsAuthenticated = criteria.IsAuthenticated,
                    CustId = criteria.CustId,
                    SubAccId = criteria.SubAccId,
                    SiteName = criteria.Site,
                    Currency = criteria.Currency,
                    ErrMsg = criteria.ErrMsg
                };
            }

            if (result.IsAuthenticated)
            {
                var cachedTime = new CacheItemOptions().SetAbsoluteExpiration(cacheTime);
                await cacheService.SetAsync(cachedKey, result, cachedTime);
            }

            return result;
        }
    }
}