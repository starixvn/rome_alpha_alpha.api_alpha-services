using System.Net;
using System.Security.Claims;
using Ext.ApiGateways.Services;

namespace Ext.ApiGateways.Infrastructure.Middlewares;

public class CheckAccountStatusMiddleware
{
    private readonly RequestDelegate next;
    private readonly ICustomerService customerService;
    private readonly Fanex.Logging.ILogger flogger;

    public CheckAccountStatusMiddleware(
        RequestDelegate next,
        ICustomerService customerService,
        Fanex.Logging.ILogger flogger)
    {
        this.next = next;
        this.customerService = customerService;
        this.flogger = flogger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        flogger.Info("CheckAccountStatus Start");
        if (context.User.Identity is not null
            && context.User.Identity.IsAuthenticated
            && int.TryParse(context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out var custId))
        {
            flogger.Info("GetCustomerStatus Start");
            var accountStatus = await customerService.GetCustomerStatus(custId);
            flogger.Info("GetCustomerStatus End");

            if (accountStatus.IsLocked)
            {
                await HandleForbiddenResponse(context, "Your account is locked");
                return;
            }

            if (!accountStatus.IsActive)
            {
                await HandleForbiddenResponse(context, "Your account is closed");
                return;
            }
        }

        flogger.Info("CheckAccountStatus End");
        await next(context);
    }

    private static Task HandleForbiddenResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return context.Response.WriteAsync($"Forbidden: {message}");
    }
}