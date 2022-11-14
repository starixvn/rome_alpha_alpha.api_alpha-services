using System.Net;
using Alpha.ApiKey.API;
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

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5000, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });
                options.Listen(IPAddress.Any, 5001, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
        })
        .UseSerilog(SeriLogger.Configure);