using Alpha.ApiKey.API.Models;

namespace Alpha.ApiKey.API.Services;

public interface IExtApiKeyService
{
   Task<VerifyExtApiKeyResult> VerifyApiKey(string key, string username, string password);
}
