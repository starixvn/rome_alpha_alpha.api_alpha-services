using Alpha.ApiKey.Api.Services;
using Fanex.Caching.Redis;
using GrpcApiKey;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddTransient<IApiKeyService, ApiKeyService>();
builder.Services.AddGrpc();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRedisCache(builder.Configuration.GetSection("Fanex.Caching.Redis"));

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

app.UseHttpsRedirection();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<ApiKeyGrpcService>();
});

app.MapGet("/", () => "Alpha Identity API !!!");

app.Run();