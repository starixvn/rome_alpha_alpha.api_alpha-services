
using Microsoft.AspNetCore.Mvc;
using GrpcReports;
using Ext.ApiGateways.Services;
using Common.Logging.Extensions;
using Alpha.Utilities.Models;
using System.Net;
using System.Security.Claims;
using Ext.ApiGateways.Constants;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]/{product=All}")]
[ApiController]
public class OutstandingController : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<OutstandingController> logger;

    public OutstandingController(
        Reports.ReportsClient apiClient,
        ICustomerService customerService,
        ILogger<OutstandingController> logger)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index(
        [FromRoute] string product)
    {
        if (!ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = string.Empty };
        }

        logger.LogRequest("Outstanding", Request.Headers, new[] { "Key", "Username" });

        if (!int.TryParse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int custId))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);

        if (viewerInfo.RoleId == UserLevelModel.Member)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = viewerInfo.CustId,
            CustLevelId = viewerInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = viewerInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            CustName = string.Empty,
            ProductGroup = product
        };

        if (viewerInfo.RoleId == UserLevelModel.Super)
        {
            return await SuperOutstanding(reportRequest);
        }

        if (viewerInfo.RoleId == UserLevelModel.Master)
        {
            return await MasterOutstanding(reportRequest);
        }

        return await AgentOutstanding(reportRequest);
    }

    private async Task<IActionResult> SuperOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetSuperOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetMasterOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetAgentOutstandingAsync(request);

        return Json(result);
    }
}