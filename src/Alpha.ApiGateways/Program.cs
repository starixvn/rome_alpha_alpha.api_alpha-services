using System.Reflection;
using Alpha.ApiGateways.Extensions;
using Alpha.ApiGateways.Middlewares;
using Alpha.ApiGateways.RateLimit.Middlewares;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddFanexLogging(configuration)
    .AddOneBookUm(configuration)
    .AddCustomMVC(configuration)
    .AddCustomAuthentication()
    .AddCustomAuthorization()
    .AddApplicationServices()
    .AddHealthChecks();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Alpha API Gateways", Version = "v1" });
    options.CustomSchemaIds(type => type.FullName);
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
    app.ConfigureExceptionHandler();
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<CheckAccountStatusMiddleware>();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health").AllowAnonymous();
});

app.UseSwagger();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Alpha API Gateways v1"));

app.Run();