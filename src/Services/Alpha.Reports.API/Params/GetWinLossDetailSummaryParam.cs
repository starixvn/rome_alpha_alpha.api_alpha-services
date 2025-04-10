using Alpha.ApiKey.API.Models;
using Alpha.Utilities.Constants;
using Alpha.Utilities.Models;
using GrpcReports;

namespace Alpha.Reports.API.Params;

public class GetWinLossDetailSummaryParam
{
    public GetWinLossDetailSummaryParam(GetWinLossDetailSummaryRequest request, ReportsAppSettingsModel settings)
    {
        CustId = request.CustId;
        CustLevel = (byte)request.CustLevelId;
        Site = request.Site;

        DateTime dt;
        FromDate = DateTime.TryParse(request.FromDate, out dt) ? dt : DateTime.MinValue;
        ToDate = DateTime.TryParse(request.ToDate, out dt) ? dt : DateTime.MinValue;
        ProductNames = string.IsNullOrWhiteSpace(request.ProductNames) ? ProductGroupConstants.All : request.ProductNames;
        ProductDetails = string.IsNullOrWhiteSpace(request.ProductDetails) ? ProductGroupConstants.All : request.ProductDetails;
        Username = request.Username;
        Level = request.Level;
        Language = string.IsNullOrWhiteSpace(request.Language) ? "en-US" : request.Language;

        var threeYearsAgo = DateTime.Today.AddYears(-3);
        MinDate = new DateTime(threeYearsAgo.Year, 1, 1);
        Settings = settings;
    }

    public int CustId { get; }
    public byte CustLevel { get; }
    public string Site { get; }

    public DateTime FromDate { get; }
    public DateTime ToDate { get; }
    public string ProductNames { get; }
    public string ProductDetails { get; }
    public string Username { get; }
    public int Level { get; }
    public string Language { get; }

    public DateTime MinDate { get; }
    public ReportsAppSettingsModel Settings { get; }

    public bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Site) && IsValidCustLevel() && IsValidFromToDate();

    private bool IsValidCustLevel() => UserLevelModel.Member < CustLevel && CustLevel <= UserLevelModel.Super;

    private bool IsValidFromToDate()
    {
        const int GapBetweenFromToDate = 92;

        return MinDate <= FromDate && FromDate <= ToDate
            && ToDate.Subtract(FromDate).TotalDays <= GapBetweenFromToDate;
    }
}