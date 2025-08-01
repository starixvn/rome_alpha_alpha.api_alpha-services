using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Alpha.ApiGateways.Requests;

public class WinLossDetailRequest
{
    private const int MaxDateRangeInDays = 31;

    [FromBody]
    public string CustName { get; set; }

    [FromBody]
    [JsonRequired]
    public DateTimeOffset? FromDate { get; set; } = DateTimeOffset.Now.AddDays(-MaxDateRangeInDays).Date;

    [FromBody]
    [JsonRequired]
    public DateTimeOffset? ToDate { get; set; } = DateTimeOffset.Now.Date;

    [FromBody]
    [JsonRequired]
    public int? Level { get; set; }

    public bool ValidateDateRange()
    {
        var fromDate = FromDate ?? DateTimeOffset.MinValue;
        var toDate = ToDate ?? DateTimeOffset.MinValue;

        var currentDate = DateTimeOffset.Now.Date;
        var minDate = currentDate.AddDays(-MaxDateRangeInDays);
        var minFromDate = toDate.AddDays(-MaxDateRangeInDays);

        return toDate >= fromDate
            && fromDate >= minDate
            && fromDate >= minFromDate;
    }
}