using GrpcCustomer;
using Microsoft.AspNetCore.Mvc;
using Common.Logging.Extensions;

namespace Ext.ApiGateways.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductController : Controller
{
    private readonly Customer.CustomerClient apiClient;
    private readonly ILogger<ProductController> logger;

    public ProductController(
        Customer.CustomerClient apiClient, 
        ILogger<ProductController> logger)
    {
        this.apiClient = apiClient;
        this.logger = logger;
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> GetAll()
    {        
        logger.LogRequest("GetAllProducts", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);
        var siteNameClaim = User.Claims.FirstOrDefault(x => x.Type == "SiteName");
        var currencyClaim = User.Claims.FirstOrDefault(x => x.Type == "Currency");

        if (siteNameClaim == null || currencyClaim == null || !int.TryParse(currencyClaim.Value, out var currency))
        {
            return BadRequest("Required claims are missing or invalid.");
        }

        var request = new GetAllProductsRequest {
            SiteName = siteNameClaim.Value,
            Currency = currency
        };

        var result = await apiClient.GetAllProductsAsync(request);

        return Json(result.Products.Select(p => new Ext.ApiGateways.Models.ProductModel(p.Name, p.Code)));
    }

    [HttpPost]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> ClearCache()
    {
        logger.LogRequest("ClearProductsCache", Request.Headers, Constants.RequestLoggingConstants.RequestKeys);
        var siteNameClaim = User.Claims.FirstOrDefault(x => x.Type == "SiteName");
        var currencyClaim = User.Claims.FirstOrDefault(x => x.Type == "Currency");

        if (siteNameClaim == null || currencyClaim == null || !int.TryParse(currencyClaim.Value, out var currency))
        {
            return BadRequest("Required claims are missing or invalid.");
        }

        var request = new ClearProductsCacheRequest {
            SiteName = siteNameClaim.Value,
            Currency = currency
        };

        var result = await apiClient.ClearProductsCacheAsync(request);

        return Json(result);
    }
}