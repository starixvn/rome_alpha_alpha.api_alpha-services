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

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<GrpcExceptionInterceptor>();

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
        }).AddInterceptor<GrpcExceptionInterceptor>();

        return services;
    }
}