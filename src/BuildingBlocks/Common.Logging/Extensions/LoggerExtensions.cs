using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Logging.Extensions;

public static class LoggerExtensions
{
    public static void LogFailedAuthentication(
        this Fanex.Logging.ILogger logger,
        IHeaderDictionary headers,
        string[] keys)
    {
        try
        {
            Serilog.Context.LogContext.PushProperty("AuthenticationStatus", "Failed");

            foreach (string key in keys)
            {
                Serilog.Context.LogContext.PushProperty(key, headers[key]);
            }

            logger.Info($"AuthenticateFailed!");
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
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
}