using System.Security.Claims;
using System.Text.Encodings.Web;
using Alpha.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Alpha.ApiGateways.Middlewares;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ITokenService tokenService;

    public BasicAuthenticationHandler(ITokenService tokenService, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
    : base(options, logger, encoder, clock)
    {
        this.tokenService = tokenService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var apiKey = Request.Headers["ApiKey"].ToString();
        var passPhrase = Request.Headers["PassPhrase"].ToString();
        var apiSignature = Request.Headers["ApiSignature"].ToString();
        var timeSpan = Request.Headers["TimeSpan"].ToString();

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(passPhrase) || string.IsNullOrEmpty(apiSignature) || string.IsNullOrEmpty(timeSpan))
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        var verifyTokenResult = await tokenService.VerifyApiKey(apiKey, passPhrase, apiSignature, timeSpan);
        if (verifyTokenResult != null && verifyTokenResult.IsAuthenticated)
        {
            var claims = new List<Claim>
            {
                new Claim("scope", "super"),
            };
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
