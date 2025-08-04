namespace Alpha.ApiGateways.Helpers;

public static class IPHelper
{
    public static string[] GetClientIPs(HttpContext context)
    {
        var ip = context.Request.Headers["X-Actual-IP"];

        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = context.GetServerVariable("HTTP_X_FORWARDED_FOR");
        }

        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = context.GetServerVariable("REMOTE_ADDR");
        }

        var localIps = new[] { "localhost", "::1" };

        if (string.IsNullOrWhiteSpace(ip) || Array.Exists(localIps, s => ip.Equals(s)))
        {
            ip = "127.0.0.1";
        }

        return ip.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public static string GetClientIP(HttpContext context)
    {
        var ips = GetClientIPs(context);
        var ip = ips[0];
        return ip.Trim();
    }
}