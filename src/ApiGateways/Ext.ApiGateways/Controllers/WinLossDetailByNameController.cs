using System.Net;
using Ext.ApiGateways.Requests;
using Ext.ApiGateways.Services;
using Alpha.Utilities.Models;
using GrpcReports;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Common.Logging.Extensions;
using Alpha.Utilities.Constants;
using GrpcCustomer;
using Microsoft.Extensions.Options;
using Ext.ApiGateways.Configurations;

namespace Ext.ApiGateways.Controllers;

[ApiController]
public class WinLossDetailByNameController : BaseAPIController
{
    private readonly Reports.ReportsClient apiClient;
    private readonly ICustomerService customerService;
    private readonly ILogger<WinLossDetailByNameController> logger;
    private readonly ExtApiGatewaysAppSettings appSettings;

    public WinLossDetailByNameController(
        Reports.ReportsClient apiClient,
        ICustomerService customerService,
        ILogger<WinLossDetailByNameController> logger,
        Customer.CustomerClient customerClient,
        IOptions<ExtApiGatewaysAppSettings> appSettings) : base(customerClient)
    {
        this.apiClient = apiClient;
        this.customerService = customerService;
        this.logger = logger;
        this.appSettings = appSettings.Value;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]/{custName}/{product=All}")]
    public async Task<IActionResult> Index(
        [FromRoute] string custName,
        [FromRoute] string product,
        [FromBody] WinLossDetailRequest request)
    {
        if (!ProductGroupConstants.IsValid(product))
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.ProductIsInvalid };
        }

        logger.LogRequest("WinLossDetailByName", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);

        if (!request.ValidateDateRange())
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.DateIsInvalid };
        }

        var custId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            return BadRequest(LoggingConstants.SearchAccountMustNotBeMember);
            //throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.SearchAccountMustNotBeMember };
        }

        var reportRequest = new GetWinLossDetailReportRequest
        {
            CustName = custName,
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            FromDate = request.FromDate.Value.ToShortDateString(),
            ToDate = request.ToDate.Value.ToShortDateString(),
            ProductGroup = product,
            Level = request.Level
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
        var result = await apiClient.GetSuperWinLossDetailAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> MasterWinLossDetail(GetWinLossDetailReportRequest request)
    {
        var result = await apiClient.GetMasterWinLossDetailAsync(request);

        return Json(result);
    }

    private async Task<IActionResult> AgentWinLossDetail(GetWinLossDetailReportRequest request)
    {
        var result = await apiClient.GetAgentWinLossDetailAsync(request);

        return Json(result);
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]/{custName}")]
    public async Task<IActionResult> GetByProductCodes(
        [FromRoute] string custName,
        [FromQuery] string products,
        [FromBody] WinLossDetailRequest request)
    {
        logger.LogRequest("WinLossDetailByName", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);

        if (!request.ValidateDateRange())
        {
            throw new GrpcException { Code = (int)HttpStatusCode.BadRequest, Detail = LoggingConstants.DateIsInvalid };
        }

        var custId = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);

        var viewerInfo = await customerService.GetCustomerInfo(custId, string.Empty);
        var downlineInfo = await customerService.GetCustomerInfo(0, custName);

        if (downlineInfo.RoleId == UserLevelModel.Member)
        {
            return BadRequest(LoggingConstants.SearchAccountMustNotBeMember);
            //throw new GrpcException { Code = (int)HttpStatusCode.Unauthorized, Detail = LoggingConstants.SearchAccountMustNotBeMember };
        }

        var productIds = GetSelectedProductIds(products, appSettings.ProductList);
        var reportRequest = new GetWinLossDetailReportRequest
        {
            CustName = custName,
            CustId = downlineInfo.CustId,
            CustLevelId = downlineInfo.RoleId,
            Site = viewerInfo.Site,
            CurrencyId = downlineInfo.Currency,
            ViewerId = viewerInfo.CustId,
            ViewerLevelId = viewerInfo.RoleId,
            FromDate = request.FromDate.Value.ToShortDateString(),
            ToDate = request.ToDate.Value.ToShortDateString(),
            ProductGroup = ProductGroupConstants.All,
            ProductIds = productIds,
            Level = request.Level
        };

        return downlineInfo.RoleId switch
        {
            UserLevelModel.Super => await SuperWinLossDetail(reportRequest),
            UserLevelModel.Master => await MasterWinLossDetail(reportRequest),
            _ => await AgentWinLossDetail(reportRequest),
        };
    }
}