using Microsoft.AspNetCore.Mvc;

namespace Ext.ApiGateways.Requests;

public class WinLossDetailRequest
{
    private const int MaxDateRangeInDays = 31;

    [FromBody]
    public string FromDate { get; set; }

    [FromBody]
    public string ToDate { get; set; }

    public DateTime ActualToDate
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ToDate) || !DateTime.TryParse(ToDate, out DateTime dt))
            {
                return DateTime.Now.Date;
            }

            return dt;
        }
    }

    public DateTime ActualFromDate
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FromDate) || !DateTime.TryParse(FromDate, out DateTime dt))
            {
                return ActualToDate.AddDays(-MaxDateRangeInDays);
            }

            return dt;
        }
    }

    public bool ValidateDateRange()
    {
        var fromDate = ActualFromDate;
        var toDate = ActualToDate;

        var currentDate = DateTime.Now.Date;
        var minDate = currentDate.AddDays(-MaxDateRangeInDays);
        var minFromDate = toDate.AddDays(-MaxDateRangeInDays);

        return fromDate >= minFromDate
            && fromDate >= minDate
            && toDate >= fromDate;
    }
}