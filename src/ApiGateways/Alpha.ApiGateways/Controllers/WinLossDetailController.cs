using System.Net;
using Alpha.ApiGateways.Constants;
using Alpha.ApiGateways.Requests;
using Alpha.ApiGateways.Services;
using Alpha.Utilities.Models;
using GrpcReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Logging.Extensions;
using Alpha.Utilities.Constants;

namespace Alpha.ApiGateways.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.WinLossDetail)]
[ApiController]
public class WinLossDetailController : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly IApiKeyService apiKeyService;
    private readonly ICustomerService customerService;
    private readonly ILogger<WinLossDetailController> logger;

    public WinLossDetailController(
        Reports.ReportsClient apiClient,
        IApiKeyService apiKeyService,
        ICustomerService customerService,
        ILogger<WinLossDetailController> logger)
    {
        this.apiClient = apiClient;
        this.apiKeyService = apiKeyService;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index(
        [FromHeader] string key,
        [FromHeader] string passPhrase,
        [FromHeader] string signature,
        [FromHeader] string timestamp,
        [FromBody] WinLossDetailRequest request)
    {
        if (!request.ValidateDateRange())
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = string.Empty };
        }

        var apiKeyRequest = new ApiKeyRequest
        {
            Key = key,
            PassPhrase = passPhrase,
            Signature = signature,
            Timestamp = timestamp
        };

        logger.LogRequest("WinLossDetail", Request.Headers, new[] { "Key", "PassPhrase", "Signature", "Timestamp" });

        var apiKey = await apiKeyService.GetApiKey(apiKeyRequest.Key);
        var viewerInfo = await customerService.GetCustomerInfo(apiKey.CustId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, request.CustName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var reportRequest = new GetWinLossDetailReportRequest
        {
            CustName = request.CustName,
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            FromDate = request.FromDate.ToShortDateString(),
            ToDate = request.ToDate.ToShortDateString(),
            ProductGroup = ProductGroupConstants.All
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
        var accountStatus = await customerService.GetCustomerStatus(request.CustId);
        if (accountStatus == null || accountStatus.IsProtectedMode)
        {
            return Json(new GetSuperWinLossDetailResponse());
        }

        var result = await apiClient.GetSuperWinLossDetailAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterWinLossDetail(GetWinLossDetailReportRequest request)
    {
        var accountStatus = await customerService.GetCustomerStatus(request.CustId);
        if (accountStatus == null || accountStatus.IsProtectedMode)
        {
            return Json(new GetMasterWinLossDetailResponse());
        }

        var result = await apiClient.GetMasterWinLossDetailAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentWinLossDetail(GetWinLossDetailReportRequest request)
    {
        var accountStatus = await customerService.GetCustomerStatus(request.CustId);
        if (accountStatus == null || accountStatus.IsProtectedMode)
        {
            return Json(new GetAgentWinLossDetailResponse());
        }

        var result = await apiClient.GetAgentWinLossDetailAsync(request);

        return Json(result);
    }
}