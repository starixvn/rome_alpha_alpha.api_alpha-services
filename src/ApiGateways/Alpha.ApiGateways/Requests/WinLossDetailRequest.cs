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
    public DateTime? FromDate { get; set; } = DateTime.Now.AddDays(-MaxDateRangeInDays).Date;

    [FromBody]
    [JsonRequired]
    public DateTime? ToDate { get; set; } = DateTime.Now.Date;

    [FromBody]
    [JsonRequired]
    public int? Level { get; set; }

    public bool ValidateDateRange()
    {
        var fromDate = FromDate ?? DateTime.MinValue;
        var toDate = ToDate ?? DateTime.MinValue;

        var currentDate = DateTime.Now.Date;
        var minDate = currentDate.AddDays(-MaxDateRangeInDays);
        var minFromDate = toDate.AddDays(-MaxDateRangeInDays);

        return toDate >= fromDate
            && fromDate >= minDate
            && fromDate >= minFromDate;
    }
}