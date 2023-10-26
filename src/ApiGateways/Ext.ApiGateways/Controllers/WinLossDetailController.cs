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

[Route("api/[controller]/{product=All}")]
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
        [FromRoute] string product,
        [FromBody] WinLossDetailRequest request)
    {
        if (!ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.ProductIsInvalid };
        }

        logger.LogRequest("WinLossDetail", Request.Headers, new[] { "Key", "Username" });

        if (!request.ValidateDateRange())
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.DateIsInvalid };
        }

        var custId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);

        if (viewerInfo.RoleId == UserLevelModel.Member)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.ViewAccountMustNotBeMember };
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
            FromDate = request.FromDate.Value.ToShortDateString(),
            ToDate = request.ToDate.Value.ToShortDateString(),
            ProductGroup = product
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