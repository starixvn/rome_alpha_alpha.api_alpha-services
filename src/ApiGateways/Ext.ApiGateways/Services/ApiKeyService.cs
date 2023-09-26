using Ext.ApiGateways.Criteria;
using Ext.ApiGateways.Models;
using Fanex.Data.Repository;
using Ext.ApiGateways.Extensions;
using Fanex.Caching;

namespace Ext.ApiGateways.Services
{
    public interface IApiKeyService
    {
        Task<VerifyApiKeyResult> VerifyApiKey(string key, string username, string password);
    }

    public class ApiKeyService : IApiKeyService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IAesService aesService;
        private readonly ICacheService cacheService;

        public ApiKeyService(
            IDynamicRepository dynamicRepository,
            IAesService aesService,
            ICacheService cacheService)
        {
            this.dynamicRepository = dynamicRepository;
            this.aesService = aesService;
            this.cacheService = cacheService;
        }

        public async Task<VerifyApiKeyResult> VerifyApiKey(string key, string username, string password)
        {
            var cachedKey = $"{key}_{username}";
            var result = await cacheService.GetAsync<VerifyApiKeyResult>(cachedKey);
            if (result == null)
            {
                var rawKey = aesService.Decrypt(key);
                var rawUsername = aesService.Decrypt(username);
                var rawPassword = aesService.Decrypt(password);
                var hashPassword = PasswordHashExtensions.Hash(rawPassword);

                var criteria = new VerifyApiKeyCriteria(rawKey, rawUsername, hashPassword);
                await dynamicRepository.ExecuteAsync(criteria);

                result = new VerifyApiKeyResult
                {
                    IsAuthenticated = criteria.IsAuthenticated,
                    CustId = criteria.CustId,
                    SubAccId = criteria.SubAccId
                };
            }

            if (result.IsAuthenticated)
            {
                var cachedTime = new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                await cacheService.SetAsync(cachedKey, result, cachedTime);
            }

            return result;
        }
    }
}