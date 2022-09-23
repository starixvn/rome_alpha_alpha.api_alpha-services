using Alpha.Identity.API.Services;
using Fanex.Caching.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<IApiKeyService, ApiKeyService>();


// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRedisCache(builder.Configuration.GetSection("Fanex.Caching.Redis"));

var app = builder.Build();

app.MapControllers();

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

app.MapGet("/", () => "Alpha Identity API !!!");

app.Run();