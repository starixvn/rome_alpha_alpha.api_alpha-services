using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class OutstandingByNameRequest
{
    [FromBody]
    public string CustName { get; set; }
}