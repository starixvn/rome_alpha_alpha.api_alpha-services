namespace Alpha.Customer.API.Configurations;

public sealed class CustomerAppSettings
{
    public int CustomerStatusCacheInSeconds { get; init; }
    public int ProductCacheInSeconds { get; init; }
}
