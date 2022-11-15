using FanexLibs.Logging;
using Alpha.ApiGateways.Extensions;
using Alpha.ApiGateways.Infrastructure.Middlewares;

namespace Alpha.ApiGateways;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddFanexLogging(Configuration)
            .AddOneBookUm(Configuration)
            .AddCustomMVC(Configuration)
            .AddCustomAuthentication()
            .AddCustomAuthorization()
            .AddApplicationServices()
            .AddHealthChecks();
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

        app.UseSwagger().UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Alpha API Gateways v1");
        });

        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<CheckAccountStatusMiddleware>();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health").AllowAnonymous();
        });
    }
}