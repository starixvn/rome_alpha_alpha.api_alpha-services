using Alpha.ApiGateways.Constants;
using Alpha.ApiGateways.Requests;
using Alpha.ApiGateways.Services;
using Alpha.Utilities.Constants;
using Alpha.Utilities.Models;
using Common.Logging.Extensions;
using GrpcReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.ApiGateways.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.WinLossDetail)]
[ApiController]
public class WinLossDetailController : Controller
{
    private const int MaxAllowedLevel = 2;
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
        if (!IsValidRequest(request, out IActionResult errorResult))
        {
            return errorResult;
        }

        var apiKeyRequest = new ApiKeyRequest
        {
            Key = key,
            PassPhrase = passPhrase,
            Signature = signature,
            Timestamp = timestamp
        };

        logger.LogRequest("WinLossDetail", Request.Headers, RequestLoggingConstants.RequestKeys);

        var apiKey = await apiKeyService.GetApiKey(apiKeyRequest.Key);
        var viewerInfo = await customerService.GetCustomerInfo(apiKey.CustId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, request.CustName);

        var downlineValidationResult = ValidateDownlineInfo(downlineInfo);
        if (downlineValidationResult != null)
        {
            return downlineValidationResult;
        }

        var accountStatus = await customerService.GetCustomerStatus(downlineInfo.CustId);
        if (accountStatus == null || accountStatus.IsProtectedMode)
        {
            return Forbid(LoggingConstants.SearchAccountMustNotBeProtectedMode);
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
            FromDate = request.FromDate?.Date.ToString("yyyy-MM-dd"),
            ToDate = request.ToDate?.Date.ToString("yyyy-MM-dd"),
            ProductGroup = ProductGroupConstants.All,
            Level = request.Level ?? 0
        };

        return await GetWinLossDetailByRole(downlineInfo.RoleId, reportRequest);
    }

    private bool IsValidRequest(WinLossDetailRequest request, out IActionResult errorResult)
    {
        errorResult = null;
        if (request.Level < 0 || MaxAllowedLevel < request.Level)
        {
            errorResult = BadRequest(LoggingConstants.InvalidLevel);
            return false;
        }

        if (!request.ValidateDateRange())
        {
            errorResult = BadRequest(LoggingConstants.DateIsInvalid);
            return false;
        }

        return true;
    }

    private IActionResult ValidateDownlineInfo(dynamic downlineInfo)
    {
        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            return Unauthorized(LoggingConstants.SearchAccountMustNotBeMember);
        }
        return null;
    }

    private async Task<IActionResult> GetWinLossDetailByRole(int roleId, GetWinLossDetailReportRequest reportRequest)
    {
        return roleId switch
        {
            UserLevelModel.Super => await SuperWinLossDetail(reportRequest),
            UserLevelModel.Master => await MasterWinLossDetail(reportRequest),
            _ => await AgentWinLossDetail(reportRequest),
        };
    }

    private async Task<IActionResult> SuperWinLossDetail(GetWinLossDetailReportRequest request)
        => Json(await apiClient.GetSuperWinLossDetailAsync(request));


    private async Task<IActionResult> MasterWinLossDetail(GetWinLossDetailReportRequest request)
        => Json(await apiClient.GetMasterWinLossDetailAsync(request));


    private async Task<IActionResult> AgentWinLossDetail(GetWinLossDetailReportRequest request)
        => Json(await apiClient.GetAgentWinLossDetailAsync(request));
}