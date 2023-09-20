using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class TicketDetailsRequest
{
    [FromBody]
    public int TransType { get; set; } = -1;

    [FromBody]
    public int ProductId { get; set; }
}