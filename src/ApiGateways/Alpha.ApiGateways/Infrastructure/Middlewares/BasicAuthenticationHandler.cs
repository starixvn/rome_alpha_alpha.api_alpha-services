using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Alpha.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Common.Logging.Extensions;
using Alpha.Utilities.Constants;
using System.Text;
using Alpha.ApiGateways.Constants;

namespace Alpha.ApiGateways.Infrastructure.Middlewares;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyService apiKeyService;
    private readonly ILogger<BasicAuthenticationHandler> logger;
    private string failReason;

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
        var passPhrase = Request.Headers["PassPhrase"].ToString();
        var signature = Request.Headers["Signature"].ToString();
        var timestamp = Request.Headers["Timestamp"].ToString();

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(passPhrase) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(timestamp))
        {
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
            logger.LogFailedAuthentication("AuthenticateFailed: Invalid Params", Request.Headers, RequestLoggingConstants.RequestKeys);

            failReason = LoggingConstants.KeyPassPhraseSignatureTimestampIsNotProvided;
            return AuthenticateResult.Fail(failReason);
        }

        var verifyApiKeyResult = await apiKeyService.VerifyApiKey(key, passPhrase, signature, timestamp);
        if (verifyApiKeyResult != null && verifyApiKeyResult.IsAuthenticated)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, verifyApiKeyResult.CustId.ToString()),
                new Claim(ClaimTypes.Role, verifyApiKeyResult.RoleId.ToString())
            };

            foreach (var scope in verifyApiKeyResult.Scopes)
            {
                claims.Add(new Claim("scope", scope));
            }

            var identity = new ClaimsIdentity(claims, "Basic");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name)));
        }

        failReason = verifyApiKeyResult.ErrorMessage;
        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        Response.Headers.Add("WWW-Authenticate", "Basic realm=\"Realm\"");
        logger.LogFailedAuthentication($"AuthenticateFailed: {failReason}", Request.Headers, RequestLoggingConstants.RequestKeys);

        return AuthenticateResult.Fail(failReason);
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        if (failReason != null)
        {
            Response.Body.WriteAsync(Encoding.ASCII.GetBytes(failReason));
        }
        return Task.CompletedTask;
    }
}