using Alpha.ApiGateways.Middlewares;
using Alpha.ApiGateways.Services;
using Microsoft.AspNetCore.Authentication;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureAppConfiguration((hostingContext, config) =>
{
    config
    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
    .AddJsonFile("ocelotconfigs.dev.json", optional: false, reloadOnChange: true);
});

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOcelot();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IApiKeyService, ApiKeyService>();
builder.Services.AddHttpClient<IApiKeyService, ApiKeyService>();

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

app.MapControllers();

app.MapGet("/", () => "Alpha API Gateways !!!");

app.UseOcelot().Wait();

app.Run();