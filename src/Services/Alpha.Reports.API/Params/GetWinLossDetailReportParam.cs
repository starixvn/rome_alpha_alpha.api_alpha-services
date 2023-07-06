using Alpha.Utilities.Models;
using GrpcReports;
using Alpha.ApiKey.API.Models;

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
        Language = request.Language;
        Settings = settings;
        IsEnableSabaPromotion = CheckEnableSabaPromotion();

        MinDate = DateTime.Now.Date.AddMonths(-1);
        DateTime dt;
        FromDate = DateTime.TryParse(request.FromDate, out dt) ? dt : DateTime.MinValue;
        ToDate = DateTime.TryParse(request.ToDate, out dt) ? dt : DateTime.MinValue;
        SiteName = request.Site;
        ProductGroup = request.ProductGroup;
    }

    public int ViewerId { get; }
    public byte ViewerLevel { get; }
    public int CustId { get; }
    public byte CustLevel { get; }
    public string Site { get; }
    public int CurrencyId { get; }
    public string Language { get; }
    public string SiteName { get; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public ReportsAppSettingsModel Settings { get; }

    public IEnumerable<int> ProductIds { get; set; }
    public bool IsEnableSabaPromotion { get; set; }
    public string ProductGroup { get; set; }

    public DateTime MinDate { get; }

    public bool IsValid() =>
        CustLevel > UserLevelModel.Member &&
        CustId > 0 &&
        !UserLevelModel.IsUpline(CustLevel, ViewerLevel);

    private bool CheckEnableSabaPromotion()
    {
        var sites = Settings.OfferSabaPromotionSites.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
        return sites.Any(s => s == "*" || s.Equals(Site, StringComparison.OrdinalIgnoreCase));
    }
}