using Alpha.Token.Alpha.Params;
using Alpha.Token.Alpha.Services;
using Alpha.Token.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Token.API;

[Route("/api/[controller]")]
[ApiController]
public class TokensController : ControllerBase
{
    private readonly ITokenService tokenService;

    public TokensController(ITokenService tokenService)
    {
        this.tokenService = tokenService;
    }

    [HttpGet("{custId}")]
    public async Task<IEnumerable<AccessToken>> Index(int custId)
        => await tokenService.GetUserTokens(custId);

    [HttpPost]
    public async Task<string> CreateToken(CreateTokenParam param)
        => await tokenService.CreateToken(param.CustId, param.TokenName, param.Scopes, param.ExpirationDate);
}