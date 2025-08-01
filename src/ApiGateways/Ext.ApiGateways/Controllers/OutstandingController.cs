using Microsoft.AspNetCore.Mvc;
using GrpcReports;
using Ext.ApiGateways.Services;
using Common.Logging.Extensions;
using Alpha.Utilities.Models;
using System.Net;
using System.Security.Claims;
using Alpha.Utilities.Constants;
using GrpcCustomer;
using Ext.ApiGateways.Requests;

namespace Ext.ApiGateways.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutstandingController : BaseAPIController
{
    private readonly Reports.ReportsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<OutstandingController> logger;

    public OutstandingController(
        Reports.ReportsClient apiClient,
        ICustomerService customerService,
        ILogger<OutstandingController> logger,
        Customer.CustomerClient customerClient) : base(customerClient)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("{product=All}")]
    public async Task<IActionResult> Index(
        [FromRoute] string product,
        [FromBody] OutstandingRequest request)
    {
        if (!ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.ProductIsInvalid };
        }

        logger.LogRequest("Outstanding", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);
      
        var nameIdentifierClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null || string.IsNullOrEmpty(nameIdentifierClaim.Value))
        {
            return BadRequest("User identifier claim is missing.");
        }
        var custId = int.Parse(nameIdentifierClaim.Value);
        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);

        if (viewerInfo.RoleId == UserLevelModel.Member)
        {
            return BadRequest(LoggingConstants.ViewAccountMustNotBeMember);
        }

        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = viewerInfo.CustId,
            CustLevelId = viewerInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = viewerInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            CustName = string.Empty,
            ProductGroup = product,
            Level = request.Level
        };

        if (viewerInfo.RoleId == UserLevelModel.Super)
        {
            return await SuperOutstanding(reportRequest);
        }

        if (viewerInfo.RoleId == UserLevelModel.Master)
        {
            return await MasterOutstanding(reportRequest);
        }

        return await AgentOutstanding(reportRequest);
    }

    private async Task<IActionResult> SuperOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetSuperOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetMasterOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetAgentOutstandingAsync(request);

        return Json(result);
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("index")]
    public async Task<IActionResult> GetByProductCodes(
        [FromQuery] string products,
        [FromBody] OutstandingRequest request)
    {
        logger.LogRequest("Outstanding", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);

        var nameIdentifierClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (nameIdentifierClaim == null || string.IsNullOrEmpty(nameIdentifierClaim.Value))
        {
            return BadRequest("User identifier claim is missing.");
        }
        var custId = int.Parse(nameIdentifierClaim.Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);

        if (viewerInfo.RoleId == UserLevelModel.Member)
        {
            return BadRequest(LoggingConstants.ViewAccountMustNotBeMember);
        }

        var productIds = await GetSelectedProductIds(products);
        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = viewerInfo.CustId,
            CustLevelId = viewerInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = viewerInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            CustName = string.Empty,
            ProductGroup = ProductGroupConstants.All,
            ProductIds = productIds,
            Level = request.Level
        };

        if (viewerInfo.RoleId == UserLevelModel.Super)
        {
            return await SuperOutstanding(reportRequest);
        }

        if (viewerInfo.RoleId == UserLevelModel.Master)
        {
            return await MasterOutstanding(reportRequest);
        }

        return await AgentOutstanding(reportRequest);
    }

}