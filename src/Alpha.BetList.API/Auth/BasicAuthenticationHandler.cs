using System.Security.Claims;
using System.Text.Encodings.Web;
using Alpha.BetList.API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Alpha.BetList.API.Auth;

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
        var authHeader = Request.Headers["Authorization"].ToString();
        if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Basic ".Length).Trim();
            var verifyTokenResult = await tokenService.VerifyToken(token);

            if (verifyTokenResult != null && verifyTokenResult.IsAuthenticated)
            {
                var claims = new[] { new Claim("scope", string.Join(" ", verifyTokenResult.Scopes)) };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
            }

            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
        else
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }
    }
}
