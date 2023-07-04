using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class WinLossBetListRequest
{
    [FromBody]
    public DateTime? Date { get; set; }

    public bool Validate()
    {
        return Date.HasValue;
    }
}