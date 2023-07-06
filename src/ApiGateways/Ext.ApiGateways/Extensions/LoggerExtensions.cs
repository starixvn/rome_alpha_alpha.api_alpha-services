
using System.Text;
namespace Ext.ApiGateways.Extensions;

public static class LoggerExtensions
{
    public static void LogHeaders(this Fanex.Logging.ILogger logger, IHeaderDictionary headers)
    {
        var t = new StringBuilder();

        foreach (string key in headers.Keys)
        {
            t.Append(key).Append('=').Append(headers[key]).Append(", ");
        }

        logger.Info($"AuthenticateFailed: headers={t}");
    }

    public static void LogCallFunction(
        this ILogger logger, string function, Dictionary<string, string> request)
    {
        var t = new StringBuilder();

        foreach (string key in request.Keys)
        {
            t.Append(key).Append('=').Append(request[key]).Append(", ");
        }

        logger.LogInformation("{function}: {t}", function, t);
    }
}