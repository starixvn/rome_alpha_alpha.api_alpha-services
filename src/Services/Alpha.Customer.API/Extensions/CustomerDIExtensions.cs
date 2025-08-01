using System.Diagnostics.CodeAnalysis;
using Alpha.Customer.API.Configurations;
using Alpha.Customer.API.DataAccesses;
using Alpha.Customer.API.Services;

namespace Alpha.Customer.API.Extensions;

// Exclude this class from code coverage because it only contains DI registration logic.
[ExcludeFromCodeCoverage(Justification = "Contains only DI registration logic, no business logic to test.")]
public static class CustomerDIExtensions
{
    public static IServiceCollection AddCustomerServices(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<CustomerAppSettings>(configuration.GetSection(nameof(CustomerAppSettings)))
            .AddTransient<ICustomerService, CustomerService>()
            .AddTransient<ICustomerDataAccess, CustomerDataAccess>();
}
