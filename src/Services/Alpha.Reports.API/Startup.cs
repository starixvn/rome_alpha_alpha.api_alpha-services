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
using Alpha.Reports.API.DataAccesses;
using Microsoft.AspNetCore.Diagnostics;
using Alpha.Utilities.Interceptors;
using Fanex.Data.Repository;
using Fanex.Caching;

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
           .ConfigurePartialUMServices(Configuration);

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcServerExceptionInterceptor>();
            options.EnableDetailedErrors = true;
        });
        
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
                    .SetIsOriginAllowed(_ => true)
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

                    throw ex.Error;
                }
            
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

        services.AddTransient<Alpha.Shared.Services.Services.IReportDateService>(serviceProvider =>
            new Alpha.Shared.Services.Services.ReportDateService(
                serviceProvider.GetRequiredService<IDynamicRepository>(),
                serviceProvider.GetRequiredService<ICacheService>(),
                "Alpha.Reports"));

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

        services.AddGrpc(c => c.Interceptors.Add<CheckBelongingInterceptor>());
    }
}