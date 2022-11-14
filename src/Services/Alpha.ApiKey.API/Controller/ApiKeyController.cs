using Microsoft.AspNetCore.Mvc;
using Alpha.ApiKey.API.Params;
using Alpha.ApiKey.API.Services;
using Alpha.ApiKey.API.Models;
using System.Net;

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

    [HttpGet("{custId:int}")]
    [ProducesResponseType(typeof(IEnumerable<ApiKeyEntity>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<ApiKeyEntity>>> GetApiKeysByCustId(int custId)
    {
        var apiKeys = await apiKeyService.GetApiKeys(custId);
        return Ok(apiKeys);
    }

    [HttpGet("{key}")]
    [ProducesResponseType(typeof(ApiKeyEntity), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiKeyResult>> GetApiKey(string key)
    {
        var apiKey = await apiKeyService.Get(key);
        return Ok(apiKey);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ActionResult<ApiKeyResult>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ApiKeyResult>> CreateApiKey(CreateApiKeyParam param)
    {
        var result = await apiKeyService.CreateApiKey(param.CustId, param.ApiName, param.PassPhrase, param.Scopes, param.ExpirationDate);
        return Ok(result);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<ActionResult> UpdateApiKey([FromBody] UpdateApiKeyParam param)
    {
        var apiKey = await apiKeyService.Get(param.ApiKey);
        if (apiKey == null)
        {
            return NotFound();
        }

        if (apiKey.CustId != param.CustId)
        {
            return BadRequest();
        }

        await apiKeyService.UpdateApiKey(param.CustId, param.ApiKey, param.ApiName, param.Scopes, param.ExpirationDate);
        return Ok();
    }


    [HttpDelete("{key}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> DeleteApiKey(string key)
    {
        var apiKey = await apiKeyService.Get(key);
        if (apiKey == null)
        {
            return NotFound();
        }

        await apiKeyService.RevokeApiKey(key, apiKey.CustId);
        return NoContent();
    }
}