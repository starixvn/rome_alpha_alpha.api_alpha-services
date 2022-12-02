using Alpha.ApiGateways.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GrpcReports;

namespace Alpha.ApiGateways.Controllers;

[Route("api/[controller]")]
[Authorize(Policy = PolicyConstants.Outstanding)]
[ApiController]
public class OutstandingController : Controller
{
    private readonly Reports.ReportsClient apiClient;

    public OutstandingController(Reports.ReportsClient apiClient)
    {
        this.apiClient = apiClient;
    }

    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Index([FromBody] GetOutstandingReportRequest request)
    {
        var result = await apiClient.GetOutstandingReportAsync(request);

        return Json(result);
    }
}