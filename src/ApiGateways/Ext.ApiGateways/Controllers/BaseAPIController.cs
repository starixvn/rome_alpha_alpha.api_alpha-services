using GrpcCustomer;
using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Controllers;

[ApiController]
public class BaseAPIController : Controller
{
    private readonly Customer.CustomerClient customerClient;

    public BaseAPIController(Customer.CustomerClient customerClient)
    {
        this.customerClient = customerClient;
    }

    protected async Task<string> GetSelectedProductIds(string codes)
    {
        var request = new GetAllProductsRequest();
        var response = await customerClient.GetAllProductsAsync(request);
        var products = response.Products ?? Enumerable.Empty<ProductModel>();

        if (string.IsNullOrWhiteSpace(codes) ||
            string.Equals("All", codes, StringComparison.OrdinalIgnoreCase))
        {
            return string.Join(',', products.Select(p => p.Id));
        }

        var cs = codes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var ids = new List<int>();
        foreach (var c in cs)
        {
            var sp = products.FirstOrDefault(p => string.Equals(p.Code, c, StringComparison.OrdinalIgnoreCase));
            if (sp != null)
            {
                ids.Add(sp.Id);
            }
        }

        return ids.Any() ? string.Join(',', ids) : "-1";
    }
}