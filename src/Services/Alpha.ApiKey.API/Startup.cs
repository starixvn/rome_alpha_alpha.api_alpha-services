using Alpha.ApiKey.API.Models;
using Alpha.ApiKey.API.Services;
using Alpha.Utilities.Interceptors;
using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;
using FanexLibs.Logging;
using GrpcApiKey;
using GrpcExtApiKey;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Alpha.ApiKey.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<APIKeyAppSettings>(Configuration.GetSection("APIKeyAppSettings"));
        services
            .AddFanexLogging(Configuration)
            .AddFanexSql(Configuration)
            .AddSingleton<IDynamicRepository, DynamicRepository>()
            .AddSingleton<ICacheService, CacheService>()
            .AddTransient<IApiKeyService, ApiKeyService>()
            .AddTransient<IExtApiKeyService, ExtApiKeyService>();

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcServerExceptionInterceptor>();
            options.EnableDetailedErrors = true;
        });

        services
            .AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        services
            .AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .SetIsOriginAllowed(_ => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            })
            .AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiKey.API", Version = "v1" }));
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

        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiKey.API v1"));

        app.UseSerilogRequestLogging();

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<ApiKeyGrpcService>();
            endpoints.MapGrpcService<ExtApiKeyGrpcService>();
            endpoints.MapGet("/", () => $"{typeof(Startup).Assembly.GetName().Name} is running!");
        });
    }
}