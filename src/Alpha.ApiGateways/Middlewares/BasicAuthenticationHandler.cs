using System.Security.Claims;
using System.Text.Encodings.Web;
using Alpha.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Alpha.ApiGateways.Middlewares;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyService apiKeyService;

    public BasicAuthenticationHandler(IApiKeyService apiKeyService, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
    : base(options, logger, encoder, clock)
    {
        this.apiKeyService = apiKeyService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var key = Request.Headers["Key"].ToString();
        var passPhrase = Request.Headers["PassPhrase"].ToString();
        var signature = Request.Headers["Signature"].ToString();
        var timeSpan = Request.Headers["TimeSpan"].ToString();

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(passPhrase) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timeSpan))
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        var verifyApiKeyResult = await apiKeyService.VerifyApiKey(key, passPhrase, signature, timeSpan);
        if (verifyApiKeyResult != null && verifyApiKeyResult.IsAuthenticated)
        {
            var claims = new List<Claim>
            {
                new Claim("CustId", verifyApiKeyResult.CustId.ToString()),
                new Claim("RoleId", verifyApiKeyResult.RoleId.ToString())
            };
            foreach (var scope in verifyApiKeyResult.Scopes)
            {
                claims.Add(new Claim("scope", scope));
            }
            var identity = new ClaimsIdentity(claims, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }
        else
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }
}
