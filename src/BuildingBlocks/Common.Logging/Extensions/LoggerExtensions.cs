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
            logger.LogError(e.Message);
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

            logger.LogInformation("Request: " + functionName);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
    }

    public static void LogAccountIsProtectedMode(this ILogger logger)
    {
        logger.LogError("Account status is protected mode");
    }
}