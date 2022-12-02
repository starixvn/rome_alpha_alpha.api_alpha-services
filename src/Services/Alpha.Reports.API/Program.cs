using System.Net;
using Alpha.Reports.API;
using Common.Logging;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
var appName = typeof(Program).Assembly.GetName().Name;

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", appName);
    var host = CreateHostBuilder(args).Build();
    Log.Information("Starting web host ({ApplicationContext})...", appName);
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", appName);
}
finally
{
    Log.CloseAndFlush();
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("./App_Data/PartialUM.json", optional: true)
        .AddCommandLine(args)
        .Build();

    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, int.Parse(config["ReportsAppSettings:Http1AndHttp2Port"]), listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
                options.Listen(IPAddress.Any, int.Parse(config["ReportsAppSettings:Http2Port"]), listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
        })
        .UseSerilog(SeriLogger.Configure);

    return builder;
}