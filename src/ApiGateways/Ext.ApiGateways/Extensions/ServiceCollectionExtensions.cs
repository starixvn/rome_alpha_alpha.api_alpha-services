
using Ext.ApiGateways.Infrastructure.Filters;
using Ext.ApiGateways.Infrastructure.Middlewares;
using Ext.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using GrpcCustomer;
using GrpcReports;
using Ext.ApiGateways.Configurations;
using Microsoft.Extensions.Options;
using Ext.ApiGateways.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Threading.RateLimiting;
using System.Globalization;
using Ext.ApiGateways.Infrastructure.RateLimits;
using GrpcBetLists;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Ext.ApiGateways.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOneBookUm(this IServiceCollection services, IConfiguration configuration)
    {
        var umConfig = configuration.GetSection("ONEbook.UM").Get<UMClientConfiguration>();
        services.AddSingleton(umConfig);
        services.AddSingleton<IPHelper, IPHelperImpl>();
        services.AddSingleton<IMaintenanceService, MaintenanceService>();
        services.AddSingleton<IUMCacheService, UMCacheService>();
        services.AddSingleton<IUMRestClient, UMRestClient>();

        return services;
    }

    public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<UrlsConfig>(configuration.GetSection(UrlsConfig.Urls));

        services.AddControllers(options => options.Filters.Add<CheckUMFilter>())
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ext API Gateways", Version = "v1" });
            options.CustomSchemaIds(type => type.FullName);
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return services;
    }

    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

        return services;
    }

    public static IServiceCollection AddCustomRateLimiter(this IServiceCollection services, IConfiguration configuration)
    {
        var ipRateLimitConfig = new IPRateLimitConfig();
        configuration.GetSection(IPRateLimitConfig.IpRateLimit).Bind(ipRateLimitConfig);
        services.Configure<CustomerRateLimitConfig>(configuration.GetSection(CustomerRateLimitConfig.CustomerRateLimit));

        services.AddRateLimiter(options =>
        {
            options.OnRejected = (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                return new ValueTask();
            };

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var remoteIp = httpContext.Connection.RemoteIpAddress.ToString();
                    return RateLimitPartition.GetSlidingWindowLimiter(remoteIp, partition => new SlidingWindowRateLimiterOptions
                    {
                        AutoReplenishment = ipRateLimitConfig.AutoReplenishment,
                        PermitLimit = ipRateLimitConfig.PermitLimit,
                        Window = ipRateLimitConfig.WindowByUnit,
                        SegmentsPerWindow = ipRateLimitConfig.SegmentsPerWindow
                    });
                })
                );
            options.AddPolicy<string, CustomRateLimiterPolicy>("CustomRateLimit");
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<GrpcExceptionInterceptor>();
        services.AddTransient<ReportsGrpcExceptionInterceptor>();

        services.AddScoped<IAesService, AesService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();

        services.AddTransient<ICustomerService, CustomerService>();

        services.AddGrpcClient<Customer.CustomerClient>((services, options) =>
        {
            var customerApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcCustomer;
            options.Address = new Uri(customerApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        services.AddGrpcClient<Reports.ReportsClient>((services, options) =>
        {
            var api = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcReports;
            options.Address = new Uri(api);
        }).AddInterceptor<ReportsGrpcExceptionInterceptor>();

        services.AddGrpcClient<BetLists.BetListsClient>((services, options) =>
        {
            var api = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcBetLists;
            options.Address = new Uri(api);

        })
        .ConfigureChannel(c =>
        {
            c.MaxReceiveMessageSize = null;
            c.MaxSendMessageSize = null;
        })
        .AddInterceptor<ReportsGrpcExceptionInterceptor>();

        services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver());

        return services;
    }
}