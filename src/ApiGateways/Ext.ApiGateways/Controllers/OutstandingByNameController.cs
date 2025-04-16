
using Microsoft.AspNetCore.Mvc;
using Ext.ApiGateways.Services;
using Common.Logging.Extensions;
using Alpha.Utilities.Models;
using System.Net;
using System.Security.Claims;
using Alpha.Utilities.Constants;
using GrpcReports;
using GrpcCustomer;
using Ext.ApiGateways.Requests;

namespace Ext.ApiGateways.Controllers;

[ApiController]
public class OutstandingByNameController : BaseAPIController
{
    private readonly Reports.ReportsClient reportsClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<OutstandingByNameController> logger;

    public OutstandingByNameController(
        Reports.ReportsClient reportsClient,
        ICustomerService customerService,
        ILogger<OutstandingByNameController> logger,
        Customer.CustomerClient customerClient) : base(customerClient)
    {
        this.reportsClient = reportsClient;
        this.customerService = customerService;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]/{custName}/{product=All}")]
    public async Task<IActionResult> Index(
        [FromRoute] string custName,
        [FromRoute] string product,
        [FromBody] OutstandingRequest request)
    {
        if (!ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.ProductIsInvalid };
        }

        logger.LogRequest("OutstandingByName", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);

        var custId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            return BadRequest(LoggingConstants.SearchAccountMustNotBeMember);
            //throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.SearchAccountMustNotBeMember };
        }

        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            CustName = custName,
            ProductGroup = product,
            Level = request.Level
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
        var result = await reportsClient.GetSuperOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> SuperOnMasterOutstanding(GetOutstandingReportRequest request)
    {
        var result = await reportsClient.GetSuperOnMasterOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> SuperOnAgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await reportsClient.GetSuperOnAgentOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterOutstanding(GetOutstandingReportRequest request)
    {
        var result = await reportsClient.GetMasterOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterOnAgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await reportsClient.GetMasterOnAgentOutstandingAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentOutstanding(GetOutstandingReportRequest request)
    {
        var result = await reportsClient.GetAgentOutstandingAsync(request);

        return Json(result);
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]/{custName}")]
    public async Task<IActionResult> GetByProductCodes(
        [FromRoute] string custName,
        [FromQuery] string products,
        [FromBody] OutstandingRequest request)
    {
        logger.LogRequest("OutstandingByName", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);

        var custId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            return BadRequest(LoggingConstants.SearchAccountMustNotBeMember);
            //throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.SearchAccountMustNotBeMember };
        }

        var productIds = await GetSelectedProductIds(products);
        var reportRequest = new GetOutstandingReportRequest
        {
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            CustName = custName,
            ProductGroup = ProductGroupConstants.All,
            ProductIds = productIds,
            Level = request.Level
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
}