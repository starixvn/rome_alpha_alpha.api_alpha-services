using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Logging.Extensions;

public static class LoggerExtensions
{
    public static void LogFailedAuthentication(
        this ILogger logger, string message, IHeaderDictionary headers, string[] keys)
    {
        try
        {
            Serilog.Context.LogContext.PushProperty("AuthenticationStatus", "Failed");

            foreach (string key in keys)
            {
                Serilog.Context.LogContext.PushProperty(key, headers[key]);
            }

            logger.LogInformation(message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occurred in LogFailedAuthentication");
        }
    }

    public static void LogRequest(this ILogger logger, string functionName, IHeaderDictionary headers, string[] keys)
    {
        try
        {
            Serilog.Context.LogContext.PushProperty("AuthenticationStatus", "Pass");

            foreach (string key in keys)
            {
                Serilog.Context.LogContext.PushProperty(key, headers[key]);
            }

            logger.LogInformation("Request: {FunctionName}", functionName);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occurred in LogRequest");
        }
    }

    public static void LogRateLimit(this ILogger logger, string message, IHeaderDictionary headers, string[] keys)
    {
        try
        {
            Serilog.Context.LogContext.PushProperty("StatusCode", "429");

            foreach (string key in keys)
            {
                Serilog.Context.LogContext.PushProperty(key, headers[key]);
            }

            logger.LogInformation(message);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occurred in LogRateLimit");
        }
    }
}