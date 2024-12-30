using Microsoft.AspNetCore.Mvc;

namespace Alpha.ApiGateways.Requests;

public class OutstandingRequest
{
    [FromBody]
    public string CustName { get; set; }

    [FromBody]
    public int Level { get; set; }
}