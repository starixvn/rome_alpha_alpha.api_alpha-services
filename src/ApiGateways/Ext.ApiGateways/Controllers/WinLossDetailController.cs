using System.Net;
using Ext.ApiGateways.Models;
using Ext.ApiGateways.Requests;
using Ext.ApiGateways.Services;
using Alpha.Utilities.Models;
using GrpcReports;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WinLossDetailController : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<WinLossDetailController> logger;

    public WinLossDetailController(
        Reports.ReportsClient apiClient,
        ICustomerService customerService,
        ILogger<WinLossDetailController> logger)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index(
        [FromHeader] string key,
        [FromHeader] string username,
        [FromHeader] string password,
        [FromBody] WinLossDetailRequest request)
    {
        if (!request.ValidateDateRange())
        {
            throw new ReportsException { Code = (int)HttpStatusCode.UnavailableForLegalReasons, Detail = string.Empty };
        }

        logger.LogInformation("WinLossDetailByName - {key}: {request}", key, request);

        if (!int.TryParse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int custId))
        {
            throw new ReportsException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);

        if (viewerInfo.RoleId == UserLevelModel.Member)
        {
            throw new ReportsException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var reportRequest = new GetWinLossDetailReportRequest
        {
            CustName = string.Empty,
            CustId = viewerInfo.CustId,
            CustLevelId = viewerInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = viewerInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            FromDate = request.FromDate.ToShortDateString(),
            ToDate = request.ToDate.ToShortDateString()
        };

        return viewerInfo.RoleId switch
        {
            UserLevelModel.Super => await SuperWinLossDetail(reportRequest),
            UserLevelModel.Master => await MasterWinLossDetail(reportRequest),
            _ => await AgentWinLossDetail(reportRequest),
        };
    }

    private async Task<IActionResult> SuperWinLossDetail(GetWinLossDetailReportRequest request)
    {
        var result = await apiClient.GetSuperWinLossDetailAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterWinLossDetail(GetWinLossDetailReportRequest request)
    {
        var result = await apiClient.GetMasterWinLossDetailAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentWinLossDetail(GetWinLossDetailReportRequest request)
    {
        var result = await apiClient.GetAgentWinLossDetailAsync(request);

        return Json(result);
    }
}