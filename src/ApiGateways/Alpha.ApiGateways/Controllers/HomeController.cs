using Microsoft.AspNetCore.Mvc;

namespace Alpha.ApiGateways.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => new RedirectResult("~/swagger");
}