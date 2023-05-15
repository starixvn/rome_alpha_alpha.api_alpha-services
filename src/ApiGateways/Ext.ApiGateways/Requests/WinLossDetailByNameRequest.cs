using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class WinLossDetailByNameRequest : WinLossDetailRequest
{
    [FromBody]
    public string CustName { get; set; }
}