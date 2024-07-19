using GrpcCustomer;
using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Controllers;

[ApiController]
public class BaseAPIController : Controller
{
    public readonly Customer.CustomerClient customerClient;
    
    public BaseAPIController(Customer.CustomerClient customerClient)
    {
        this.customerClient = customerClient;
    }

    protected string GetSelectedProductIds(string codes, IEnumerable<Ext.ApiGateways.Models.ProductModel> productList) {
        var request = new GetAllProductsRequest();
        var products = productList ?? Enumerable.Empty<Ext.ApiGateways.Models.ProductModel>();

        if(string.IsNullOrWhiteSpace(codes) || 
            string.Equals("All", codes, StringComparison.OrdinalIgnoreCase)) {
            return string.Join(',', products.Select(p => p.Id));
        }

        var cs = codes.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
        var ids = new List<int>();
        foreach(var c in cs) {
            var sp = products.FirstOrDefault(p => string.Equals(p.Code, c, StringComparison.OrdinalIgnoreCase));
            if(sp != null) {
                ids.Add(sp.Id);
            };
        }

        return ids.Any() ? string.Join(',', ids) : "-1";
    }
}