using Alpha.ApiKey.API.Models;
using Alpha.Reports.API.Extensions;
using Alpha.Reports.API.Services;
using FanexLibs.Data;
using FanexLibs.Logging;
using GrpcReports;
using Microsoft.OpenApi.Models;
using Alpha.PartialUM;
using GrpcCustomer;
using Microsoft.Extensions.Options;
using Alpha.Reports.API.Interceptors;
using Fanex.RemotingRef;
using Alpha.Reports.API.DataAccesses;
using Microsoft.AspNetCore.Diagnostics;

namespace Alpha.Reports.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
           .AddFanexData(Configuration)
           .AddFanexLogging(Configuration)
           .AddFanexCachingMemory()
           .ConfigurePartialUMServices(Configuration)
           .AddGrpc();

        AddReportDI(services);

        services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddHealthChecks();

        services
            .AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            })
            .AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "Reports.API", Version = "v1" }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
            app.ConfigureExceptionHandler();
        }

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var ex = context.Features.Get<IExceptionHandlerFeature>();

                if (ex != null && ex.Error != null)
                {
                    var logger = app.ApplicationServices.GetService<Fanex.Logging.ILogger>();

                    await logger.ErrorAsync("Error happens", ex.Error);
                }

                throw ex.Error;
            });
        });


        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reports.API v1"));

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<ReportGrpcService>();
            endpoints.MapGet("/", () => $"{typeof(Startup).Assembly.GetName().Name} is running!");
            endpoints.MapHealthChecks("/health").AllowAnonymous();
        });
    }

    private void AddReportDI(IServiceCollection services)
    {
        services.Configure<ReportsAppSettingsModel>(Configuration.GetSection("ReportsAppSettings"));
        services.AddTransient<IWinLossDetailDataAccess, WinLossDetailDataAccess>();

        services.AddSingleton<IRemotingRefService, RemotingRefService>();
        services.AddTransient<ICacheRefService, CacheRefService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IAutoFollowBetService, AutoFollowBetService>();
        services.AddTransient<IReportDateService, ReportDateService>();

        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<IProductDetailService, ProductDetailService>();
        services.AddTransient<IOutstandingService, OutstandingService>();
        services.AddTransient<IWinLossDetailService, WinLossDetailService>();

        services.AddTransient<GrpcExceptionInterceptor>();
        services.AddGrpcClient<Customer.CustomerClient>((services, options) =>
        {
            var customerApi = services.GetRequiredService<IOptions<ReportsAppSettingsModel>>().Value.GrpcCustomer;
            options.Address = new Uri(customerApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        RefDataTypeContainer
            .ConnectTo(new Uri(Configuration["ReportsAppSettings:RemoteRefURL"]))
            .FromApplication("Alpha.Reports.Site");

        services.AddGrpc(c => c.Interceptors.Add<CheckBelongingInterceptor>());
    }
}