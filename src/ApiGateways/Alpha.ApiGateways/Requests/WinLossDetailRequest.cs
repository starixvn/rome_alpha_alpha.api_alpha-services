using Microsoft.AspNetCore.Mvc;

namespace Alpha.ApiGateways.Requests;

public class WinLossDetailRequest
{
    [FromBody]
    public string CustName { get; set; }
}