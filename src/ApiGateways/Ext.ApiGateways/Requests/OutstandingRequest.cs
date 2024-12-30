using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class OutstandingRequest
{
    [FromBody]
    public string CustName { get; set; }

    [FromBody]
    public int Level { get; set; }
}