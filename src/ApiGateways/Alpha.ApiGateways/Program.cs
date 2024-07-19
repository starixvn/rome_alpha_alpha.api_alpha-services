using Alpha.ApiGateways;
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
    => Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("common-appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls("http://0.0.0.0:5000");
        })
        .UseSerilog(SeriLogger.Configure, true);