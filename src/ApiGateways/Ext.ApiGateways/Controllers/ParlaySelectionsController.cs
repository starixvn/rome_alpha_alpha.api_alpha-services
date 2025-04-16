
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
public class ParlaySelectionsController : Controller
{
    private readonly BetLists.BetListsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<ParlaySelectionsController> logger;

    public ParlaySelectionsController(
        BetLists.BetListsClient apiClient,
        ICustomerService customerService,
        ILogger<ParlaySelectionsController> logger)
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

        logger.LogRequest("ParlaySelections", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);


        var custId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId != UserLevelModel.Member)
        {
            return BadRequest(LoggingConstants.SearchAccountMustBeMember);
            //throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.SearchAccountMustBeMember };
        }

        var isBelonging = await customerService.GetBelongingInfo(custId, downlineInfo.CustId, 0, refNo);

        if (!isBelonging)
        {
            throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.TicketMustBelongToMember };
        }

        var getParlaySelectionsRequest = new GetParlaySelectionsRequest
        {
            DetailKey = request.DetailKey,
            RefNo = refNo
        };

        var result = await apiClient.ParlaySelectionsAsync(getParlaySelectionsRequest);
        if (string.IsNullOrEmpty(result.Selections))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.DetailKeyIsInvalid };
        }
        return Json(result);
    }
}