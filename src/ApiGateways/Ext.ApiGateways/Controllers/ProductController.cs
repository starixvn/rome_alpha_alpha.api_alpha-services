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
        var siteName = User.Claims.FirstOrDefault(x => x.Type == "SiteName").Value;
        var currency = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "Currency").Value);
        var request = new GetAllProductsRequest {
            SiteName = siteName,
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
        var siteName = User.Claims.FirstOrDefault(x => x.Type == "SiteName").Value;
        var currency = int.Parse(User.Claims.FirstOrDefault(x => x.Type == "Currency").Value);
        var request = new ClearProductsCacheRequest {
            SiteName = siteName,
            Currency = currency
        };

        var result = await apiClient.ClearProductsCacheAsync(request);

        return Json(result);
    }
}