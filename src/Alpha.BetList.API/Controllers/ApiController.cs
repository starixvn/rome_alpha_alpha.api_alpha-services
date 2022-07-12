using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.BetList.API.Controllers;

[Route("api")]
public class ApiController : ControllerBase
{
    [HttpGet("private-scoped")]
    [Authorize("read:messages")]
    public IActionResult Scoped()
    {
        return Ok(new
        {
            Message = "Hello from a private endpoint! You need to be authenticated and have a scope of read:messages to see this."
        });
    }
}