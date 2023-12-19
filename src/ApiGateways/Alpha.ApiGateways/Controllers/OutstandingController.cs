using Alpha.ApiGateways.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GrpcReports;
using Alpha.ApiGateways.Services;
using Alpha.ApiGateways.Requests;
using Alpha.Utilities.Models;
using System.Net;
using Common.Logging.Extensions;
using Alpha.Utilities.Constants;

namespace Alpha.ApiGateways.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.Outstanding)]
[ApiController]
public class OutstandingController : Controller
{
    private readonly Reports.ReportsClient apiClient;
    private readonly IApiKeyService apiKeyService;
    private readonly ICustomerService customerService;
    private readonly ILogger<OutstandingController> logger;

    public OutstandingController(
        Reports.ReportsClient apiClient,
        IApiKeyService apiKeyService,
        ICustomerService customerService,
        ILogger<OutstandingController> logger)
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

        logger.LogRequest("Outstanding", Request.Headers, RequestLoggingConstants.RequestKeys);

        var apiKey = await apiKeyService.GetApiKey(apiKeyRequest.Key);
        var viewerInfo = await customerService.GetCustomerInfo(apiKey.CustId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, request.CustName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.SearchAccountMustNotBeMember };
        }

        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            CustName = request.CustName,
            ProductGroup = ProductGroupConstants.All
        };

        var accountStatus = await customerService.GetCustomerStatus(downlineInfo.CustId);
        if (accountStatus == null || accountStatus.IsProtectedMode)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Forbidden, Detail = LoggingConstants.SearchAccountMustNotBeProtectedMode };
        }

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
        => Json(await apiClient.GetSuperOutstandingAsync(request));


    private async Task<IActionResult> SuperOnMasterOutstanding(GetOutstandingReportRequest request)
        => Json(await apiClient.GetSuperOnMasterOutstandingAsync(request));


    private async Task<IActionResult> SuperOnAgentOutstanding(GetOutstandingReportRequest request)
        => Json(await apiClient.GetSuperOnAgentOutstandingAsync(request));


    private async Task<IActionResult> MasterOutstanding(GetOutstandingReportRequest request)
        => Json(await apiClient.GetMasterOutstandingAsync(request));


    private async Task<IActionResult> MasterOnAgentOutstanding(GetOutstandingReportRequest request)
        => Json(await apiClient.GetMasterOnAgentOutstandingAsync(request));


    private async Task<IActionResult> AgentOutstanding(GetOutstandingReportRequest request)
        => Json(await apiClient.GetAgentOutstandingAsync(request));

}