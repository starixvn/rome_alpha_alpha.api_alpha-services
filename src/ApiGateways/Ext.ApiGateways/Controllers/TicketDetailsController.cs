
using Microsoft.AspNetCore.Mvc;
using Ext.ApiGateways.Services;
using Alpha.Utilities.Models;
using System.Net;
using System.Security.Claims;
using GrpcBetLists;
using Common.Logging.Extensions;
using Ext.ApiGateways.Requests;
using Alpha.Utilities.Constants;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]/{custName}/{refNo}")]
[ApiController]
public class TicketDetailsController : Controller
{
    private readonly BetLists.BetListsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<TicketDetailsController> logger;

    public TicketDetailsController(
        BetLists.BetListsClient apiClient,
        ICustomerService customerService,
        ILogger<TicketDetailsController> logger)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Index(
        [FromRoute] string custName,
        [FromRoute] long refNo,
        [FromBody] TicketDetailsRequest request)
    {

        logger.LogRequest("TicketDetails", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);

        var custId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId != UserLevelModel.Member)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.SearchAccountMustBeMember };
        }


        var getTicketDetailsRequest = new GetTicketDetailsRequest
        {
            RefNo = refNo,
            CustId = downlineInfo.CustId,
            TransType = request.ExtraInfo.TransType,
            ProductId = request.ExtraInfo.ProductId,
            DetailKey = request.DetailKey ?? string.Empty
        };

        return viewerInfo.RoleId switch
        {
            UserLevelModel.Super => await SuperViewTicketDetails(getTicketDetailsRequest),
            UserLevelModel.Master => await MasterViewTicketDetails(getTicketDetailsRequest),
            _ => await AgentViewTicketDetails(getTicketDetailsRequest),
        };
    }

    private async Task<IActionResult> SuperViewTicketDetails(GetTicketDetailsRequest request)
    {
        var result = await apiClient.SuperViewTicketDetailsAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterViewTicketDetails(GetTicketDetailsRequest request)
    {
        var result = await apiClient.MasterViewTicketDetailsAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentViewTicketDetails(GetTicketDetailsRequest request)
    {
        var result = await apiClient.AgentViewTicketDetailsAsync(request);

        return Json(result);
    }
}