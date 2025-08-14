using FanexLibs.Logging;
using Alpha.ApiGateways.Extensions;
using Alpha.ApiGateways.Infrastructure.Middlewares;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.HttpLogging;
using Alpha.Utilities.Models;
using Alpha.Utilities.Configurations;
using System.Globalization;

namespace Alpha.ApiGateways;

public class Startup
{
    private const int HttpBodyLogLimit = 4096;

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
            .AddCustomRateLimiter(Configuration)
            .AddCustomAuthentication()
            .AddCustomAuthorization()
            .AddApplicationServices()
            .AddDistributedMemoryCache()
            .AddHealthChecks();

        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders;
            logging.RequestHeaders.Add("x-forwarded-for");
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = HttpBodyLogLimit;
            logging.ResponseBodyLogLimit = HttpBodyLogLimit;
        });
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
                if (ex == null)
                {
                    return;
                }

                if (ex.Error is GrpcException info)
                {
                    context.Response.StatusCode = info.Code;
                    context.Response.ContentType = Text.Plain;
                    await context.Response.WriteAsync(info.Detail);
                    return;
                }

                throw ex.Error;
            });
        });

        app.UseSwagger(c =>
        {
            c.PreSerializeFilters.Add((swagger, _) =>
            {
                swagger.Servers = new List<OpenApiServer> {
                    new OpenApiServer { Url = Configuration.GetSection(UrlsConfig.Urls).Get<UrlsConfig>().ApiServer }
                };
            });
        });

        if (env.IsDevelopment())
        {
            app.UseSwaggerUI(options =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "." : "..";
                options.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Alpha API Gateways v1");
            });
        }

        app.UseHttpLogging();

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseWhen(context => !context.Request.Path.Value.ToLower(CultureInfo.InvariantCulture).Trim().Contains("/health", StringComparison.InvariantCulture), applicationBuilder =>
        {
            applicationBuilder.UseRateLimiter();
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseAuthorization();
            applicationBuilder.UseMiddleware<CheckAccountStatusMiddleware>();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers().RequireAuthorization();
            endpoints.MapHealthChecks("/health").AllowAnonymous().DisableRateLimiting();
        });
    }
}