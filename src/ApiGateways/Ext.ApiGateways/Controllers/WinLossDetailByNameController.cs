using System.Net;
using Ext.ApiGateways.Requests;
using Ext.ApiGateways.Services;
using Alpha.Utilities.Models;
using GrpcReports;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Common.Logging.Extensions;
using Alpha.Utilities.Constants;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]/{custName}/{product=All}")]
[ApiController]
public class WinLossDetailByNameController : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<WinLossDetailByNameController> logger;

    public WinLossDetailByNameController(
        Reports.ReportsClient apiClient,
        ICustomerService customerService,
        ILogger<WinLossDetailByNameController> logger)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index(
        [FromRoute] string custName,
        [FromRoute] string product,
        [FromBody] WinLossDetailRequest request)
    {
        if (!ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = string.Empty };
        }

        logger.LogRequest("WinLossDetailByName", Request.Headers, new[] { "Key", "Username" });

        if (!request.ValidateDateRange())
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = string.Empty };
        }

        if (!int.TryParse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int custId))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var reportRequest = new GetWinLossDetailReportRequest
        {
            CustName = custName,
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            FromDate = request.FromDate.Value.ToShortDateString(),
            ToDate = request.ToDate.Value.ToShortDateString(),
            ProductGroup = product
        };

        return downlineInfo.RoleId switch
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