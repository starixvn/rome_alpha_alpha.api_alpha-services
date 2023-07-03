using System.Net;
using System.Security.Claims;
using Ext.ApiGateways.Services;

namespace Ext.ApiGateways.Infrastructure.Middlewares;

public class CheckAccountStatusMiddleware
{
    private readonly RequestDelegate next;
    private readonly ICustomerService customerService;

    public CheckAccountStatusMiddleware(RequestDelegate next, ICustomerService customerService)
    {
        this.next = next;
        this.customerService = customerService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity is not null
            && context.User.Identity.IsAuthenticated
            && int.TryParse(context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value, out var custId))
        {
            var accountStatus = await customerService.GetCustomerStatus(custId);

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

        await next(context);
    }

    private static Task HandleForbiddenResponse(HttpContext context, string message)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return context.Response.WriteAsync($"Forbidden: {message}");
    }
}