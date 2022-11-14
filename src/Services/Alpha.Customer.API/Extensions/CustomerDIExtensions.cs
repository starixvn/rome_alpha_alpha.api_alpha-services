using System.Diagnostics.CodeAnalysis;
using Alpha.Customer.API.Configurations;
using Alpha.Customer.API.Services;

namespace Alpha.Customer.API.Extensions;

[ExcludeFromCodeCoverage]
public static class CustomerDIExtensions
{
    public static IServiceCollection AddCustomerServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<CustomerAppSettings>(configuration.GetSection(nameof(CustomerAppSettings)))
            .AddTransient<ICustomerService, CustomerService>();
}
