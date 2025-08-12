using Alpha.Shared.Services.Contracts;

namespace Alpha.Shared.Services.Interceptors;

public static class BelongingCheckHelper
{
    public static bool ShouldSkipBelongingCheck<TRequest>(TRequest request)
    {
        return request == null;
    }

    public static bool IsBaseRequestValid(IBaseRequest baseRequest)
    {
        return baseRequest != null && baseRequest.ViewerId != 0 && !string.IsNullOrWhiteSpace(baseRequest.CustName);
    }

    public static string FormatBelongingErrorMessage(IBaseRequest baseRequest)
    {
        return $"{baseRequest.CustName} is not belonging to {baseRequest.ViewerId}";
    }
}
