using Ext.ApiGateways.Models;

namespace Ext.ApiGateways.Configurations;

public sealed class ExtApiGatewaysAppSettings
{
    public IEnumerable<ProductModel> ProductList { get; init; }
}
