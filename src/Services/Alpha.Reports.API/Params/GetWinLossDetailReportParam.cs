using Alpha.Utilities.Models;
using GrpcReports;
using Alpha.Reports.API.Constants;
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
        PageSize = request.PageSize == 0 ? DefaultVariables.PageSize : request.PageSize;
        PageIndex = request.PageIndex == 0 ? DefaultVariables.PageIndex : request.PageIndex;
        IsEnableSabaPromotion = CheckEnableSabaPromotion();
    }

    public int ViewerId { get; }
    public byte ViewerLevel { get; }
    public int CustId { get; }
    public byte CustLevel { get; }
    public string Site { get; }
    public int CurrencyId { get; }
    public string Language { get; }

    public int PageSize { get; }
    public int PageIndex { get; }
    public ReportsAppSettingsModel Settings { get; }

    public IEnumerable<int> ProductIds { get; set; }
    public bool IsEnableSabaPromotion { get; set; }

    public DateTime MinDate { get; }

    public bool IsValid() => CustId > 0 && !UserLevelModel.IsUpline(CustLevel, ViewerLevel);

    private bool CheckEnableSabaPromotion()
    {
        var sites = Settings.OfferSabaPromotionSites.Split(new[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
        return sites.Any(s => s == "*" || s.Equals(Site, StringComparison.OrdinalIgnoreCase)); ;
    }
}