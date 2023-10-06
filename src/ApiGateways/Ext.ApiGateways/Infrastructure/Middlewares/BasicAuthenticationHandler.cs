using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Ext.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Common.Logging.Extensions;

namespace Ext.ApiGateways.Infrastructure.Middlewares;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyService apiKeyService;
    private readonly ILogger<BasicAuthenticationHandler> logger;

    public BasicAuthenticationHandler(
        IApiKeyService apiKeyService,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        ISystemClock clock,
        ILogger<BasicAuthenticationHandler> logger)
        : base(options, loggerFactory, encoder, clock)
    {
        this.apiKeyService = apiKeyService;
        this.logger = logger;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var key = Request.Headers["Key"].ToString();
        var username = Request.Headers["Username"].ToString();
        var password = Request.Headers["Password"].ToString();

        if (string.IsNullOrEmpty(key)
         || string.IsNullOrEmpty(username)
         || string.IsNullOrEmpty(password))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");

            logger.LogFailedAuthentication("AuthenticateFailed: Invalid Params", Request.Headers, new[] { "Key", "Username" });
            return AuthenticateResult.Fail("Bad Request");
        }

        var verifyApiKeyResult = await apiKeyService.VerifyApiKey(key, username, password);

        if (verifyApiKeyResult != null && verifyApiKeyResult.IsAuthenticated)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, verifyApiKeyResult.CustId.ToString()),
                new Claim("SubAccId", verifyApiKeyResult.SubAccId.ToString()),
            };

            var identity = new ClaimsIdentity(claims, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }

        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
        logger.LogFailedAuthentication($"AuthenticateFailed: {verifyApiKeyResult?.ErrMsg}", Request.Headers, new[] { "Key", "Username" });

        return AuthenticateResult.Fail("Unauthorized");
    }
}
