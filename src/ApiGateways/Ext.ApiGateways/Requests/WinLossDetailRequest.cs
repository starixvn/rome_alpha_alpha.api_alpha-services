using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class WinLossDetailRequest
{
    private const int MaxDateRangeInDays = 31;

    [FromBody]
    public string CustName { get; set; }

    [FromBody]
    public DateTime? FromDate { get; set; }
    [FromBody]
    public DateTime? ToDate { get; set; }

    public bool ValidateDateRange()
    {
        if (!FromDate.HasValue || !ToDate.HasValue)
        {
            return false;
        }

        var fromDate = FromDate.Value;
        var toDate = ToDate.Value;

        var currentDate = DateTime.Now.Date;
        var minDate = currentDate.AddDays(-MaxDateRangeInDays);
        var minFromDate = toDate.AddDays(-MaxDateRangeInDays);

        return toDate >= fromDate
            && fromDate >= minDate
            && fromDate >= minFromDate;
    }
}