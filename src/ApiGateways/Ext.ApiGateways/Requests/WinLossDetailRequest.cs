using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class WinLossDetailRequest
{
    private const int MaxDateRangeInDays = 31;

    [FromBody]
    public string CustName { get; set; }

    [FromBody]
    public DateTime FromDate { get; set; } = DateTime.Now.AddDays(-MaxDateRangeInDays).Date;

    [FromBody]
    public DateTime ToDate { get; set; } = DateTime.Now.Date;

    public bool ValidateDateRange()
    {
        var fromDate = FromDate;
        var toDate = ToDate;

        var currentDate = DateTime.Now.Date;
        var minDate = currentDate.AddDays(-MaxDateRangeInDays);
        var minFromDate = toDate.AddDays(-MaxDateRangeInDays);

        return toDate >= fromDate
            && fromDate >= minDate
            && fromDate >= minFromDate;
    }
}