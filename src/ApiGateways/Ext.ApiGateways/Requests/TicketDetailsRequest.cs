using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class TicketDetailsRequest
{
    [FromBody]
    public ExtraInfo ExtraInfo { get; set; } = new ExtraInfo();
}