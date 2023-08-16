using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.HttpContextData;

namespace Common.Logging;

public static class SeriLogger
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
        (context, configuration) =>
        {
            var logStackUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
            var category = context.Configuration.GetValue<string>("ElasticConfiguration:Category");
            var environment = context.Configuration.GetValue<string>("ElasticConfiguration:Env");

            configuration
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithHttpContextData()
                .WriteTo.Console()
                .WriteTo.Http(requestUri: logStackUri, queueLimitBytes: null)
                .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
                .Enrich.WithProperty("Category", category)
                .Enrich.WithProperty("Environment", environment);
        };
}