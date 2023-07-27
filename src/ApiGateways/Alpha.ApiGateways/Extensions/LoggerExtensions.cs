
using System.Text;

namespace Alpha.ApiGateways.Extensions;

public static class LoggerExtensions
{
    public static void LogHeaders(this Fanex.Logging.ILogger logger, IHeaderDictionary headers)
    {
        var t = new StringBuilder();

        foreach (var pair in headers)
        {
            t.Append(pair.Key).Append('=').Append(pair.Value).Append(", ");
        }

        logger.Info($"AuthenticateFailed: headers={t}");
    }

    public static void LogCallFunction(
        this ILogger logger, string function, Dictionary<string, object> request)
    {
        var t = new StringBuilder();

        foreach (string key in request.Keys)
        {
            t.Append(key).Append('=').Append(request[key]).Append(", ");
        }

        logger.LogInformation("{function}: {t}", function, t);
    }
}