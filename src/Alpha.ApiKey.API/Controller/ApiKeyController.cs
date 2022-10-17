using Microsoft.AspNetCore.Mvc;
using Alpha.ApiKey.API.Params;
using Alpha.ApiKey.API.Services;
using Alpha.ApiKey.API.Models;

namespace Alpha.ApiKey.API.Controller;

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
        => await apiKeyService.CreateApiKey(param.CustId, param.RoleId, param.ApiName, param.PassPhrase, param.Scopes, param.ExpirationDate, param.RestrictIps);
}