using Alpha.ApiGateways.Models;

namespace Alpha.ApiGateways.Services;

public interface ITokenService
{
    Task<VerifyTokenResult?> VerifyToken(string token);
}

public class TokenService : ITokenService
{
    private readonly HttpClient httpClient;

    public TokenService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
        this.httpClient.BaseAddress = new Uri("https://localhost:7047");
    }

    public async Task<VerifyTokenResult?> VerifyToken(string token)
        => await httpClient.GetFromJsonAsync<VerifyTokenResult>($"/api/tokens/verify/{token}");

    // public async Task<VerifyTokenResult?> VerifyToken1(string apiKey, string passPhrase, string token)
    // {

    //     return await httpClient.PostAsJsonAsync<VerifyTokenResult>($"/api/tokens/verify/{token}", );
    // }
}