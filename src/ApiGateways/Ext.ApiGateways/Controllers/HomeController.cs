using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => new RedirectResult("~/swagger");
}