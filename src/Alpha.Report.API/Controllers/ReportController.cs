using Microsoft.AspNetCore.Mvc;

namespace Alpha.Report.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class ReportController : ControllerBase
    {
        [HttpGet("reports")]
        public IActionResult GetReports()
        {
            return Ok(new string("Reports"));
        }
    }
}