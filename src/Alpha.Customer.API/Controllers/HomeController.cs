
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Customer.API.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return new RedirectResult("~/swagger");
    }
}