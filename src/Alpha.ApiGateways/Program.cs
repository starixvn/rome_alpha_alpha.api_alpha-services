using Alpha.ApiGateways.Extensions;
using Alpha.ApiGateways.Filters;
using Alpha.ApiGateways.Middlewares;
using Alpha.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config
    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile("ocelotconfigs.dev.json", optional: false, reloadOnChange: true);
});

// Add services to the container.
builder.Services.AddOneBookUm(configuration);
builder.Services.AddControllers(options =>
{
    options.Filters.Add<CheckUMFilter>();
});

builder.Services.AddOcelot();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IApiKeyService, ApiKeyService>();
builder.Services.AddHttpClient<IApiKeyService, ApiKeyService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(configuration["IdentityUrl"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute().RequireAuthorization();
    endpoints.MapHealthChecks("/health").AllowAnonymous();

});

app.UseOcelot().Wait();

app.Run();