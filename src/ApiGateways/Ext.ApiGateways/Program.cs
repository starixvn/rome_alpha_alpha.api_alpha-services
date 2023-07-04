using Ext.ApiGateways;
using Common.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
var appName = typeof(Program).Assembly.GetName().Name;

try
{
    Log.Information("Configuring web host ({ApplicationContext})...", appName);
    Log.Information("CreateHostBuilder", appName);
    var builder = CreateHostBuilder(args);

    Log.Information("Build", appName);
    var host = builder.Build();

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

    return Host.CreateDefaultBuilder(args)
         .ConfigureWebHostDefaults(webBuilder =>
         {
             webBuilder.UseStartup<Startup>();

             var port = int.Parse(config["GatewaysSettings:Port"]);
             webBuilder.UseUrls($"http://0.0.0.0:{port}");
         })
         .UseSerilog(SeriLogger.Configure);
}