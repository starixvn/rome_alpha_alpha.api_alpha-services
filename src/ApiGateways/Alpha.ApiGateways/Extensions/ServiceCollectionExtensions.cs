using Alpha.ApiGateways.Constants;
using Alpha.ApiGateways.Infrastructure.Filters;
using Alpha.ApiGateways.Infrastructure.Middlewares;
using Alpha.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using GrpcApiKey;
using GrpcCustomer;
using GrpcReports;
using Alpha.ApiGateways.Configurations;
using Microsoft.Extensions.Options;
using Alpha.ApiGateways.Infrastructure;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Threading.RateLimiting;
using System.Globalization;
using Alpha.ApiGateways.Infrastructure.RateLimits;

namespace Alpha.ApiGateways.Extensions;

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
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Alpha API Gateways", Version = "v1" });
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

    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyConstants.Outstanding, policy => policy.RequireClaim("scope", ScopeConstants.Outstanding));
            options.AddPolicy(PolicyConstants.WinLossDetail, policy => policy.RequireClaim("scope", ScopeConstants.WinLossDetail));
        });

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
                    var remoteIp = Helpers.IPHelper.GetClientIP(httpContext);
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

        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddGrpcClient<ApiKey.ApiKeyClient>((services, options) =>
        {
            var apiKeyApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcApiKey;
            options.Address = new Uri(apiKeyApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

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

        return services;
    }
}