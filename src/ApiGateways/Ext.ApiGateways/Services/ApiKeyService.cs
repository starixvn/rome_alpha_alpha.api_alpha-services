using Ext.ApiGateways.Criteria;
using Ext.ApiGateways.Models;
using Fanex.Data.Repository;
using Ext.ApiGateways.Extensions;

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

        public ApiKeyService(IDynamicRepository dynamicRepository, IAesService aesService)
        {
            this.dynamicRepository = dynamicRepository;
            this.aesService = aesService;
        }

        public async Task<VerifyApiKeyResult> VerifyApiKey(string key, string username, string password)
        {
            var rawKey = aesService.Decrypt(key);
            var rawUsername = aesService.Decrypt(username);
            var rawPassword = aesService.Decrypt(password);
            var hashPassword = PasswordHashExtensions.Hash(rawPassword);

            var criteria = new VerifyApiKeyCriteria(rawKey, rawUsername, hashPassword);
            await dynamicRepository.ExecuteAsync(criteria);

            return new VerifyApiKeyResult
            {
                IsAuthenticated = criteria.IsAuthenticated,
                CustId = criteria.CustId,
                SubAccId = criteria.SubAccId
            };
        }
    }
}