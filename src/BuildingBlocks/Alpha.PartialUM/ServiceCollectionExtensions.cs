
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Alpha.PartialUM.Models;
using Alpha.PartialUM.Services;

namespace Alpha.PartialUM;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigurePartialUMServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<PartialUMSettings>(configuration.GetSection("PartialUMSettings"));
        services.AddTransient<IPartialUMService, PartialUMService>();
        services.AddTransient<IPartialUMDataAccess, PartialUMDataAccess>();

        return services;
    }
}