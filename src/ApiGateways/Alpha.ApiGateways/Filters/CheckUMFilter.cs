using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;
using ONEbook.UM.Services;

namespace Alpha.ApiGateways.Filters;

public class CheckUMFilter : IAsyncActionFilter
{
    private readonly IMaintenanceService maintenanceService;

    public CheckUMFilter(IMaintenanceService maintenanceService)
    {
        this.maintenanceService = maintenanceService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var umInfo = await maintenanceService.GetUnderMaintenanceTimeAsync();
        if (umInfo.IsUnderMaintenanceTime)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
            var message = string.Format("Sorry, the system is under maintenance from {0} to {1} GMT+8. Please come back later.", umInfo.From, umInfo.To);
            await response.WriteAsync(message);
            return;
        }

        await next();
    }
}