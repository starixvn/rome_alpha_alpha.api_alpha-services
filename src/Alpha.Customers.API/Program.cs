using Alpha.Customers.API.Extensions;
using GrpcCustomer;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services
    .AddFanexDataWithSqlServer(configuration)
    .AddFanexLogging(configuration)
    .AddFanexCachingMemory()
    .AddCustomerServices(configuration)
    .AddControllers();

builder.Services.AddGrpc();

builder.Services.AddEndpointsApiExplorer().AddSwaggerGen();

var app = builder.Build();
var isDevelopment = app.Environment.IsDevelopment();

if (!isDevelopment)
{
    app.UseHsts();
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

// Configure the HTTP request pipeline.
app.UseSwagger().UseSwaggerUI();
app.MapControllers();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<CustomerGrpcService>();
    endpoints.MapDefaultControllerRoute();
    endpoints.MapControllers();
});


await app.RunAsync();