using Alpha.Utilities.Models;
using GrpcReports;
using Alpha.ApiKey.API.Models;
using Alpha.Utilities.Extensions;
using System.Globalization;

namespace Alpha.Reports.API.Params;

public class GetWinLossDetailReportParam
{
    public GetWinLossDetailReportParam(GetWinLossDetailReportRequest request, ReportsAppSettingsModel settings)
    {
        ViewerId = request.ViewerId;
        ViewerLevel = (byte)request.ViewerLevelId;
        CustId = request.CustId;
        CustLevel = (byte)request.CustLevelId;
        Site = request.Site;
        CurrencyId = request.CurrencyId;
        Settings = settings;

        MinDate = DateTimeOffset.Now.Date.AddMonths(-1);
        DateTimeOffset dt;
        FromDate = DateTimeOffset.TryParse(request.FromDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ? dt : DateTimeOffset.MinValue;
        ToDate = DateTimeOffset.TryParse(request.ToDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt) ? dt : DateTimeOffset.MinValue;
        SiteName = request.Site;
        ProductGroup = request.ProductGroup;
        ProductIds = request.ProductIds.ToListInt();
        Level = request.Level;
    }

    public int ViewerId { get; }
    public byte ViewerLevel { get; }
    public int CustId { get; }
    public byte CustLevel { get; }
    public string Site { get; }
    public int CurrencyId { get; }
    public string SiteName { get; }

    public DateTimeOffset FromDate { get; set; }
    public DateTimeOffset ToDate { get; set; }

    public ReportsAppSettingsModel Settings { get; }

    public IEnumerable<int> ProductIds { get; set; }
    public string ProductGroup { get; set; }

    public DateTimeOffset MinDate { get; }
    public int Level { get; }

    public bool IsValid()
    {
        bool isCustLevelValid = CustLevel > UserLevelModel.Member;
        bool isCustIdValid = CustId > 0;
        bool isNotUpline = !UserLevelModel.IsUpline(CustLevel, ViewerLevel);
        bool isLevelInRange = 0 <= Level && Level <= 2;

        return isCustLevelValid
            && isCustIdValid
            && isNotUpline
            && isLevelInRange;
    }
}