using Alpha.Customer.API.Extensions;
using Alpha.Utilities.Interceptors;
using FanexLibs.Data;
using FanexLibs.Logging;
using GrpcCustomer;
using Microsoft.OpenApi.Models;

namespace Alpha.Customer.API;

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
            .AddCustomerServices(Configuration);

        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GrpcServerExceptionInterceptor>();
            options.EnableDetailedErrors = true;
        });

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
            .AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo { Title = "Customer.API", Version = "v1" }));
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
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Customer.API v1"));

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<CustomerGrpcService>();
            endpoints.MapGet("/", () => $"{typeof(Startup).Assembly.GetName().Name} is running!");
            endpoints.MapHealthChecks("/health").AllowAnonymous();
        });
    }
}