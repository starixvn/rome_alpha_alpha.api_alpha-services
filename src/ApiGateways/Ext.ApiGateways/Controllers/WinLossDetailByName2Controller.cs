using System.Net;
using Ext.ApiGateways.Models;
using Ext.ApiGateways.Requests;
using Ext.ApiGateways.Services;
using Alpha.Utilities.Models;
using GrpcReports;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Ext.ApiGateways.Extensions;
using Ext.ApiGateways.Constants;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WinLossDetailByName2Controller : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<WinLossDetailByName2Controller> logger;
    private readonly Fanex.Logging.ILogger flogger;

    public WinLossDetailByName2Controller(
        Reports.ReportsClient apiClient,
        ICustomerService customerService,
        ILogger<WinLossDetailByName2Controller> logger,
        Fanex.Logging.ILogger flogger)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
        this.flogger = flogger;

        flogger.Info($"WinLossDetailByName2Controller Constr: {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}");
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index()
    {
        flogger.Info($"WinLossDetailByName2Controller: {DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}");
        return Json(new { });
    }
}