using Alpha.ApiGateways.Extensions;
using Alpha.ApiGateways.Models;

namespace Alpha.ApiGateways.Services;

public interface ITokenService
{
    Task<VerifyTokenResult?> VerifyApiKey(string apiKey, string passPhrase, string apiSignature, string timeSpan);
}

public class TokenService : ITokenService
{
    private readonly HttpClient httpClient;

    public TokenService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("https://localhost:7047");
    }

    public async Task<VerifyTokenResult?> VerifyApiKey(string apiKey, string passPhrase, string apiSignature, string timeSpan)
        => await httpClient.PostAsJsonAsync<VerifyTokenResult>($"/api/tokens/verify", new { apiKey, passPhrase, apiSignature, timeSpan });
}