
using Microsoft.AspNetCore.Mvc;
using Ext.ApiGateways.Services;
using Alpha.Utilities.Models;
using System.Net;
using System.Security.Claims;
using GrpcBetLists;
using Ext.ApiGateways.Constants;
using Common.Logging.Extensions;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]/{custName}/{product=All}")]
[ApiController]
public class OutstandingBetListController : Controller
{
    private readonly BetLists.BetListsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<OutstandingBetListController> logger;

    public OutstandingBetListController(
        BetLists.BetListsClient apiClient,
        ICustomerService customerService,
        ILogger<OutstandingBetListController> logger)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index(
        [FromRoute] string product,
        [FromRoute] string custName)
    {
        if (!ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = string.Empty };
        }

        logger.LogRequest("OutstandingBetList", Request.Headers, new[] { "Key", "Username" });

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

        var betListsRequest = new GetRunningBetListRequest
        {
            CustId = downlineInfo.CustId,
            ViewerId = viewerInfo.CustId,
            Site = viewerInfo.Site,
            CustName = custName,
            ProductGroup = product
        };

        return viewerInfo.RoleId switch
        {
            UserLevelModel.Super => await SuperViewRunningBetList(betListsRequest),
            UserLevelModel.Master => await MasterViewRunningBetList(betListsRequest),
            _ => await AgentViewRunningBetList(betListsRequest),
        };
    }

    private async Task<IActionResult> SuperViewRunningBetList(GetRunningBetListRequest request)
    {
        var result = await apiClient.SuperViewRunningBetListAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterViewRunningBetList(GetRunningBetListRequest request)
    {
        var result = await apiClient.MasterViewRunningBetListAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentViewRunningBetList(GetRunningBetListRequest request)
    {
        var result = await apiClient.AgentViewRunningBetListAsync(request);

        return Json(result);
    }
}