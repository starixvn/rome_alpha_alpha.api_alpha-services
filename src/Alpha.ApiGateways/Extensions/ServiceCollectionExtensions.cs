using Alpha.ApiGateways.Constants;
using Alpha.ApiGateways.Filters;
using Alpha.ApiGateways.Middlewares;
using Alpha.ApiGateways.Services;
using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.AspNetCore;
using Fanex.Logging.RabbitMQ;
using Fanex.Logging.Sentry;
using Microsoft.AspNetCore.Authentication;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using Sentry;
using GrpcApiKeyClient;
using Alpha.ApiGateways.Configurations;
using Microsoft.Extensions.Options;
using Alpha.ApiGateways.Infrastructure;

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

    public static IServiceCollection AddFanexLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var logEngines = new List<ILogEngine>
        {
            new RabbitMQLogging(configuration["Fanex.Logging:RabbitMQConnectionStrings"])
        };

        var sentryUrl = configuration["Fanex.Logging:SentryUrl"];
        if (!string.IsNullOrWhiteSpace(sentryUrl))
        {
            logEngines.Add(new SentryLogging(new SentryEngineOptions
            {
                Dsn = new Dsn(sentryUrl)
            }));
        }

        LogManager
            .SetDefaultLogCategory(configuration["Fanex.Logging:DefaultCategory"])
            .Use(logEngines);

        services.AddSingleton(Logger.Log);

        ExceptionHandler.UseConfig(new LoggingConfiguration().GetHttpHeaderInfo());

        return services;
    }

    public static IServiceCollection AddCustomMVC(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<UrlsConfig>(configuration.GetSection(UrlsConfig.Urls));

        services.AddControllers(options => options.Filters.Add<CheckUMFilter>())
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

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
            options.AddPolicy(PolicyConstants.Outstanding, policy => policy.RequireClaim("scope", "outstanding"));
            options.AddPolicy(PolicyConstants.WinLossDetail, policy => policy.RequireClaim("scope", "winloss_detail"));
        });

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddGrpcClient<ApiKey.ApiKeyClient>((services, options) =>
        {
            var apiKeyApi = services.GetRequiredService<IOptions<UrlsConfig>>().Value.GrpcApiKey;
            options.Address = new Uri(apiKeyApi);
        }).AddInterceptor<GrpcExceptionInterceptor>();

        return services;
    }
}