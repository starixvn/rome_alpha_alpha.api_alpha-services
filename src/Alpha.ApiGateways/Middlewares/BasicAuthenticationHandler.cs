using System.Security.Claims;
using System.Text.Encodings.Web;
using Alpha.ApiGateways.Models;
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
        var authHeader = Request.Headers["Authorization"].ToString();
        // var authHeader = "Basic123";
        if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
        {
            var token = authHeader.Substring("Basic ".Length).Trim();
            // var verifyTokenResult = await tokenService.VerifyToken(token);
            var verifyTokenResult = new VerifyTokenResult { IsAuthenticated = true, Scopes = new[] { "super", "master" } };

            if (verifyTokenResult != null && verifyTokenResult.IsAuthenticated)
            {
                // var claims = new[] { new Claim("scope", string.Join(" ", verifyTokenResult.Scopes)) };
                var claims = new List<Claim>
                {
                    new Claim("scope", "super"),
                    new Claim("scope1", "master")
                };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
            }

            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
            return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }
        else
        {
            Response.StatusCode = 401;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Access token is required!\"");
            return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
        }
    }
}
