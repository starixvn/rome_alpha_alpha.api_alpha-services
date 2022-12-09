using Alpha.ApiGateways.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GrpcReports;
using Alpha.ApiGateways.Services;
using Alpha.ApiGateways.Requests;
using Alpha.Utilities.Models;

namespace Alpha.ApiGateways.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.Outstanding)]
[ApiController]
public class OutstandingController : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly IApiKeyService apiKeyService;
    private readonly ICustomerService customerService;
    private readonly Fanex.Logging.ILogger logger;

    public OutstandingController(
        Reports.ReportsClient apiClient,
        IApiKeyService apiKeyService,
        ICustomerService customerService,
        Fanex.Logging.ILogger logger)
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
        [FromBody] OutstandingRequest request)
    {
        var apiKeyRequest = new ApiKeyRequest
        {
            Key = key,
            PassPhrase = passPhrase,
            Signature = signature,
            Timestamp = timestamp
        };

        logger.Info($"Get OutStanding report with key {apiKeyRequest.Key} {request}");
        var apiKey = await apiKeyService.GetApiKey(apiKeyRequest.Key);
        var viewerInfo = await customerService.GetCustomerInfo(apiKey.CustId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, request.CustName);

        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = downlineInfo.CustId,
            ViewerLevelId = downlineInfo.RoleId
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