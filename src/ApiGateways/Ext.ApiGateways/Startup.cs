using Alpha.Utilities.Configurations;
using Alpha.Utilities.Models;
using Ext.ApiGateways.Extensions;
using FanexLibs.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi.Models;
using static System.Net.Mime.MediaTypeNames;

namespace Ext.ApiGateways;

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
            .AddFanexLogging(Configuration)
            .AddFanexCachingMemory()
            .AddOneBookUm(Configuration)
            .AddCustomMVC(Configuration)
            .AddCustomRateLimiter(Configuration)
            .AddCustomAuthentication()
            .AddApplicationServices()
            .AddDistributedMemoryCache()
            .AddHealthChecks();

        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders;
            logging.RequestHeaders.Add("x-forwarded-for");
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
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

                if (ex.Error is GrpcException)
                {
                    var info = ex.Error as GrpcException;

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
            c.PreSerializeFilters.Add((swagger, httpReq) =>
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
                options.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Ext API Gateways v1");
            });
        }

        app.UseHttpLogging();

        app.UseRouting();
        app.UseCors("CorsPolicy");

        app.UseWhen(context => !context.Request.Path.Value.ToLower().Trim().Contains("/health"), applicationBuilder =>
        {
            applicationBuilder.UseRateLimiter();
            applicationBuilder.UseAuthentication();
            applicationBuilder.UseAuthorization();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers().RequireAuthorization();
            endpoints.MapHealthChecks("/health").AllowAnonymous().DisableRateLimiting();
        });
    }
}