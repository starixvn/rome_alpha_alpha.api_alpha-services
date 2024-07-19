
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
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Threading.RateLimiting;
using System.Globalization;
using GrpcBetLists;
using Alpha.Utilities.Infrastructure;
using Alpha.Utilities.Configurations;
using Alpha.Utilities.Interceptors;
using Common.Logging.Extensions;
using System.Text.Json;
using GrpcExtApiKey;

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
        services.AddRateLimiter(options =>
        {
            var logger = services.BuildServiceProvider().GetRequiredService<ILogger<RateLimitConfig>>();

            options.OnRejected = (context, cancellationToken) =>
            {
                var headers = context.HttpContext.Request.Headers;
                var rateLimiterMessage = headers.ContainsKey("RateLimiterMessage") ? headers["RateLimiterMessage"].ToString() : string.Empty;
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }
                logger.LogRateLimit(rateLimiterMessage, headers, Constants.RequestLoggingConstants.RequestKeys);

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.WriteAsJsonAsync(rateLimiterMessage);
                return new ValueTask();
            };

            options.GlobalLimiter = PartitionedRateLimiter.CreateChained(
                configuration.CreateGlobalPartitionedRateLimiter(),
                configuration.CreatePartitionedRateLimiter(RateLimitConfig.ConfigBySecond),
                configuration.CreatePartitionedRateLimiter(RateLimitConfig.ConfigByMinute),
                configuration.CreatePartitionedRateLimiter(RateLimitConfig.ConfigByHour));
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<GrpcExceptionInterceptor>();
        services.AddTransient<ReportsGrpcExceptionInterceptor>();
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddTransient<ICustomerService, CustomerService>();

        services.AddGrpcClient<ExtApiKey.ExtApiKeyClient>((services, options) =>
        {
            var apiKeyApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcApiKey;
            options.Address = new Uri(apiKeyApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

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

        services.AddControllersWithViews();
        
        services.AddMvc().AddJsonOptions(options => {
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // also the following given it's a Web API project
        services.AddControllers().AddJsonOptions(options => {
            options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }
}