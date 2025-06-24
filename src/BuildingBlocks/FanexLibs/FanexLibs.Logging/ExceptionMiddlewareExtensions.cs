using System.Text;
using Fanex.Logging.Extensions.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FanexLibs.Logging;

public static class ExceptionMiddlewareExtensions
{
    private const string BreakLine = "\r\n";
    private const string TwoBreakLine = "\r\n\r\n";

    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                Console.WriteLine("ExceptionMiddlewareExtensions");

                try
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var exception = contextFeature.Error;
                        var logger = app.ApplicationServices.GetService<Fanex.Logging.ILogger>();
                        await logger.ErrorAsync(await BuildError(context.Request, exception));

                        await ExceptionHandler.HandleAsync(exception, context).ConfigureAwait(false);
                        await context.Response.WriteAsync("Internal Server Error.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception in ExceptionHandler: {ex.Message}, {ex.StackTrace}");
                }
            });
        });
    }

    private static async Task<string> BuildError(HttpRequest request, Exception exception)
    {
        var errorBuilder = new StringBuilder();
        errorBuilder
            .Append(exception.Message).Append(TwoBreakLine)
            .Append($"PATH: {request.Scheme}://{request.Host}{request.PathBase}{request.Path}{request.QueryString}")
            .Append(TwoBreakLine)
            .Append($"BODY: {await GetRawBodyStringAsync(request)}")
            .Append(TwoBreakLine)
            .Append("HEADERS:").Append(BreakLine)
            .Append(GetHeaderValues(request))
            .Append(TwoBreakLine)
            .Append("COOKIES:").Append(BreakLine)
            .Append(GetCookiesValues(request))
            .Append(TwoBreakLine);

        errorBuilder
            .Append("EXCEPTION: ")
            .Append(exception.ToString());

        return errorBuilder.ToString();
    }

    private static async Task<string> GetRawBodyStringAsync(HttpRequest request)
    {
        request.EnableBuffering();

        if (request.Body.CanSeek)
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        return string.Empty;
    }

    private static string GetHeaderValues(HttpRequest request)
        => string.Join(BreakLine, request.Headers.Select(header => $"{header.Key}:{header.Value}"));

    private static string GetCookiesValues(HttpRequest request)
        => string.Join(BreakLine, request.Cookies.Select(header => $"{header.Key}:{header.Value}"));
}