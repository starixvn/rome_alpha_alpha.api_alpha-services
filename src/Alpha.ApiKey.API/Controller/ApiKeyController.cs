using Microsoft.AspNetCore.Mvc;
using Alpha.ApiKey.Api.Params;
using Alpha.ApiKey.Api.Services;
using Alpha.ApiKey.Api.Models;

namespace Alpha.ApiKey.Api.Controller;

[Route("/api/[controller]")]
[ApiController]
public class ApiKeyController : ControllerBase
{
    private readonly IApiKeyService apiKeyService;

    public ApiKeyController(IApiKeyService apiKeyService)
    {
        this.apiKeyService = apiKeyService;
    }

    [HttpGet("{custId}")]
    public async Task<IEnumerable<ApiKeyAccess>> Index(int custId)
        => await apiKeyService.GetUserApiKeyAccess(custId);

    [HttpPost]
    public async Task<ApiKeyResult> CreateApiKey(CreateApiKeyParam param)
        => await apiKeyService.CreateApiKey(param.CustId, param.ApiName, param.PassPhrase, param.Scopes, param.ExpirationDate);

    [HttpPost("verify")]
    public async Task<VerifyApiKeyResult> VerifyApiKey(VerifyApiKeyParam param)
        => await apiKeyService.VerifyApiKey(param.Key, param.PassPhrase, param.Signature, param.TimeSpan);
}