using Alpha.ApiGateways.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.ApiGateways.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.WinLossDetail)]
[ApiController]
public class WinLossDetailController : Controller
{
    [HttpGet]
    public IActionResult Index() => Ok();
}