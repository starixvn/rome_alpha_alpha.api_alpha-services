
using Microsoft.AspNetCore.Mvc;
using Ext.ApiGateways.Services;
using Ext.ApiGateways.Requests;
using Alpha.Utilities.Models;
using System.Net;
using System.Security.Claims;
using GrpcBetLists;
using Common.Logging.Extensions;
using Alpha.Utilities.Constants;


namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]/{custName}/{product=All}")]
[ApiController]
public class WinLossBetListController : Controller
{
    private readonly BetLists.BetListsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<WinLossBetListController> logger;

    public WinLossBetListController(
        BetLists.BetListsClient apiClient,
        ICustomerService customerService,
        ILogger<WinLossBetListController> logger)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index(
        [FromRoute] string product,
        [FromRoute] string custName,
        [FromBody] WinLossBetListRequest request)
    {
        if (!request.Validate() || !ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = string.Empty };
        }

        logger.LogRequest("WinLossBetList", Request.Headers, new[] { "Key", "Username" });

        if (!int.TryParse(this.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out int custId))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId != UserLevelModel.Member)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = string.Empty };
        }

        var betListsRequest = new GetWinLossBetListRequest
        {
            CustId = downlineInfo.CustId,
            Site = viewerInfo.Site,
            CustName = custName,
            ProductGroup = product,
            FromDate = request.Date.Value.ToShortDateString(),
            ToDate = request.Date.Value.ToShortDateString(),
            ViewerId = viewerInfo.CustId,
            RoleId = viewerInfo.RoleId
        };

        return viewerInfo.RoleId switch
        {
            UserLevelModel.Super => await SuperViewRunningBetList(betListsRequest),
            UserLevelModel.Master => await MasterViewRunningBetList(betListsRequest),
            _ => await AgentViewRunningBetList(betListsRequest),
        };
    }

    private async Task<IActionResult> SuperViewRunningBetList(GetWinLossBetListRequest request)
    {
        var result = await apiClient.SuperViewWinLossBetListAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterViewRunningBetList(GetWinLossBetListRequest request)
    {
        var result = await apiClient.MasterViewWinLossBetListAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentViewRunningBetList(GetWinLossBetListRequest request)
    {
        var result = await apiClient.AgentViewWinLossBetListAsync(request);

        return Json(result);
    }
}