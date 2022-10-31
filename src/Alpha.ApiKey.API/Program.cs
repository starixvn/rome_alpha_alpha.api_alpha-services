using Alpha.ApiKey.API.Extensions;
using Alpha.ApiKey.API.Services;
using GrpcApiKey;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
builder.Services.AddFanexData(configuration);
builder.Services.AddControllers();
builder.Services.AddTransient<IApiKeyService, ApiKeyService>();
builder.Services.AddGrpc();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    builder.WebHost.UseUrls("http://0.0.0.0:5000");
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<ApiKeyGrpcService>();
});

app.MapGet("/", () => $"{typeof(Program).Assembly.GetName().Name} is running");

app.Run();