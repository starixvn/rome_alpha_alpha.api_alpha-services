using Alpha.ApiGateways.Extensions;
using Alpha.ApiGateways.Models;

namespace Alpha.ApiGateways.Services;

public interface IApiKeyService
{
    Task<VerifyApiKeyResult?> VerifyApiKey(string key, string passPhrase, string signature, string timeSpan);
}

public class ApiKeyService : IApiKeyService
{
    private readonly HttpClient httpClient;

    public ApiKeyService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("https://localhost:7047");
    }

    public async Task<VerifyApiKeyResult?> VerifyApiKey(string key, string passPhrase, string signature, string timeSpan)
        => await httpClient.PostAsJsonAsync<VerifyApiKeyResult>($"/api/apikey/verify", new { key, passPhrase, signature, timeSpan });
}