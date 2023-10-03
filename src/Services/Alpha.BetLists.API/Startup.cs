
using Alpha.BetLists.API.Extensions;
using Alpha.BetLists.API.Services;
using FanexLibs.Data;
using FanexLibs.Logging;
using Microsoft.OpenApi.Models;
using Alpha.PartialUM;
using GrpcCustomer;
using Microsoft.Extensions.Options;
using Alpha.BetLists.API.Interceptors;
using Microsoft.AspNetCore.Diagnostics;
using GrpcBetLists;
using Alpha.BetLists.API.Models;
using Fanex.BetList.Core.Builder;
using Fanex.BetList.Core.Factories;
using Fanex.BetList.Core.Entities;
using Fanex.BetList.Core.TicketHelper;
using Fanex.RemotingRef;
using Fanex.BetList.Core.RemotingRef;
using Alpha.IPInfo.Services;
using Alpha.BetLists.API.Repositories;
using Alpha.IPInfo.DataAccess;
using Alpha.IPInfo.Models;
using Alpha.Utilities.Interceptors;

namespace Alpha.BetLists.API;

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

        AddDIs(services);

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
            .AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "BetLists.API", Version = "v1" }));
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
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "BetLists.API v1"));

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<BetListsGrpcService>();
            endpoints.MapGet("/", () => $"{typeof(Startup).Assembly.GetName().Name} is running!");
            endpoints.MapHealthChecks("/health").AllowAnonymous();
        });
    }

    private void AddDIs(IServiceCollection services)
    {
        services.Configure<BetListsAppSettingsModel>(Configuration.GetSection(BetListsAppSettingsModel.ConfigName));
        services.Configure<TicketDetailsMapperModel>(Configuration.GetSection("TicketDetailsMapper"));

        services.AddSingleton<IPInformationConfig>((_) =>
        {
            var config = Configuration.GetSection("IPInformationConfig").Get<IPInformationConfig>();

            return config;
        });

        services.AddSingleton<IRemotingRefService, RemotingRefService>();
        services.AddTransient<ICacheRef, CacheRef>();

        services.AddTransient<ICacheRefService, CacheRefService>();
        services.AddTransient<IProductService, ProductService>();
        services.AddTransient<IProductDetailService, ProductDetailService>();

        services.AddTransient<IIPInfoDataAccess, IPInfoDataAccess>();
        services.AddTransient<IIPInfoService, IPInfoService>();

        services.AddTransient<ICasinoDetailsService, CasinoDetailsService>();

        services.AddTransient<GrpcExceptionInterceptor>();
        services.AddGrpcClient<Customer.CustomerClient>((services, options) =>
        {
            var customerApi = services.GetRequiredService<IOptions<BetListsAppSettingsModel>>().Value.GrpcCustomer;
            options.Address = new Uri(customerApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        services.AddGrpc(c => c.Interceptors.Add<CheckBelongingInterceptor>());

        services.AddTransient<ITicketHelper, TicketHelper>();
        services.AddTransient<IBetListFactory<BetListHTMLBuilder>, BetListHTMLFactory>();
        services.AddTransient<IBetListHTMLBuilder>((s) => s.GetService<IBetListFactory<BetListHTMLBuilder>>().CreateBuilder());
        RefDataTypeContainer
            .ConnectTo(new Uri(Configuration.GetSection("BetListsAppSettings").Get<BetListsAppSettingsModel>().RemoteRefURL))
            .FromApplication("Alpha.Reports.Site");

        services.AddTransient<IReportDateService, ReportDateService>();
        services.AddTransient<IRunningBetListService, RunningBetListService>();
        services.AddTransient<IWinLossBetListService, WinLossBetListService>();
    }
}