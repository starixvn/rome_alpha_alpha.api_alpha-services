
using Microsoft.AspNetCore.Mvc;
using GrpcReports;
using Ext.ApiGateways.Services;
using Ext.ApiGateways.Requests;
using Alpha.Utilities.Models;
using Ext.ApiGateways.Models;
using System.Net;
using System.Security.Claims;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]/{custName}")]
[ApiController]
public class OutstandingByNameController : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<OutstandingByNameController> logger;

    public OutstandingByNameController(
        Reports.ReportsClient apiClient,
        ICustomerService customerService,
        ILogger<OutstandingByNameController> logger)
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
        [FromRoute] string custName)
    {
        logger.LogInformation("OutStandingByName - {key}: {custName}", key, custName);

        if (!int.TryParse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int custId))
        {
            throw new ReportsException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            throw new ReportsException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            CustName = custName
        };

        if (viewerInfo.RoleId == UserLevelModel.Super && downlineInfo.RoleId == UserLevelModel.Super)
        {
            return await SuperOutstanding(reportRequest);
        }

        if (viewerInfo.RoleId == UserLevelModel.Super && downlineInfo.RoleId == UserLevelModel.Master)
        {
            return await SuperOnMasterOutstanding(reportRequest);
        }

        if (viewerInfo.RoleId == UserLevelModel.Super && downlineInfo.RoleId == UserLevelModel.Agent)
        {
            return await SuperOnAgentOutstanding(reportRequest);
        }

        if (viewerInfo.RoleId == UserLevelModel.Master && downlineInfo.RoleId == UserLevelModel.Master)
        {
            return await MasterOutstanding(reportRequest);
        }

        if (viewerInfo.RoleId == UserLevelModel.Master && downlineInfo.RoleId == UserLevelModel.Agent)
        {
            return await MasterOnAgentOutstanding(reportRequest);
        }

        return await AgentOutstanding(reportRequest);
    }

    private async Task<IActionResult> SuperOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetSuperOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> SuperOnMasterOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetSuperOnMasterOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> SuperOnAgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetSuperOnAgentOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetMasterOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterOnAgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetMasterOnAgentOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetAgentOutstandingAsync(request);

        return Json(result);
    }
}