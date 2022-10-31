using Fanex.Data;
using Fanex.Data.Repository;
using Fanex.Data.SqlServer;

namespace Alpha.ApiKey.API.Extensions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddFanexData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionStrings = configuration
            .GetSection("ConnectionStrings")
            .GetChildren()
            .ToDictionary(
                connection => connection.Key,
                connection => new ConnectionConfiguration(connection.Key, connection.Value), StringComparer.OrdinalIgnoreCase);

        DbSettingProviderManager
            .StartNewSession()
            .WithSqlServer("App_Data/SpNames.xml")
            .Use(connectionStrings)
            .Run();

        return services.AddSingleton<IDynamicRepository, DynamicRepository>();
    }
}