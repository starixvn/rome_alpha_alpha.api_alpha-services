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

static IHostBuilder CreateHostBuilder(string[] args)
{
    var config = new ConfigurationBuilder()
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: true)
       .AddCommandLine(args)
       .Build();

    var builder = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("common-appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseKestrel(options =>
            {
                options.ListenAnyIP(int.Parse(config["APIKeyAppSettings:Http1AndHttp2Port"]), listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                });

                options.ListenAnyIP(int.Parse(config["APIKeyAppSettings:Http2Port"]), listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });
            });
        })
        .UseSerilog(SeriLogger.Configure);


    return builder;
}