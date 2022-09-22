using Microsoft.AspNetCore.Mvc;
using Alpha.Identity.API.Params;
using Alpha.Identity.API.Services;
using Alpha.Identity.API.Models;

namespace Alpha.Identity.API.Controller;

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