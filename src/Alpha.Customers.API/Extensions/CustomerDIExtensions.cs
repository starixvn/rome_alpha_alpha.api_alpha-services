using System.Diagnostics.CodeAnalysis;
using Alpha.Customers.API.Configurations;
using Alpha.Customers.API.Services;

namespace Alpha.Customers.API.Extensions;

[ExcludeFromCodeCoverage]
public static class CustomerDIExtensions
{
    public static IServiceCollection AddCustomerServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<CustomerAppSettings>(configuration.GetSection(nameof(CustomerAppSettings)))
            .AddTransient<ICustomerService, CustomerService>();
}
