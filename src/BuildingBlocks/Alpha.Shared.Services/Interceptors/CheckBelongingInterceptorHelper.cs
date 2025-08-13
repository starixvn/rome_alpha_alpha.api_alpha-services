using System.Net;
using Alpha.Shared.Services.Contracts;
using Alpha.Shared.Services.Models;
using Newtonsoft.Json;

namespace Alpha.Shared.Services.Interceptors;

public static class CheckBelongingInterceptorHelper
{
    public static async Task<bool> PerformBelongingCheck<TRequest>(
        TRequest request,
        Func<int, string, Task<dynamic>> getDownlineInfoAsync)
    {
        if (BelongingCheckHelper.ShouldSkipBelongingCheck(request))
        {
            return true;
        }

        var requestJson = request?.ToString();
        if (string.IsNullOrEmpty(requestJson))
        {
            return true;
        }

        var baseRequest = JsonConvert.DeserializeObject<BaseRequest>(requestJson);
        if (baseRequest == null || !BelongingCheckHelper.IsBaseRequestValid(baseRequest))
        {
            return true;
        }

        var downlineInfo = await getDownlineInfoAsync(baseRequest!.ViewerId, baseRequest.CustName);

        return downlineInfo.IsBelonging;
    }

    public static async Task<bool> CheckBelongingWithErrorHandling<TRequest>(
        TRequest request,
        Func<int, string, Task<dynamic>> getDownlineInfoAsync,
        Func<string, Task> handleForbiddenResponse)
    {
        var isBelonging = await PerformBelongingCheck(request, getDownlineInfoAsync);

        if (!isBelonging)
        {
            var requestJson = request?.ToString();
            if (!string.IsNullOrEmpty(requestJson))
            {
                var baseRequest = JsonConvert.DeserializeObject<BaseRequest>(requestJson);
                if (baseRequest != null)
                {
                    var errorMessage = BelongingCheckHelper.FormatBelongingErrorMessage(baseRequest);
                    await handleForbiddenResponse(errorMessage);
                }
            }
        }

        return isBelonging;
    }

    public static Task CreateForbiddenResponse(dynamic context, string message)
    {
        var httpContext = context.GetHttpContext();
        httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return httpContext.Response.WriteAsync($"Forbidden: {message}");
    }
}
