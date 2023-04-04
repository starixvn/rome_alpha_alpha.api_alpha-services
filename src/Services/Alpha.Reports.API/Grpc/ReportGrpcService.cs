using Alpha.ApiKey.API.Models;
using Alpha.Reports.API.Services;
using Microsoft.Extensions.Options;

namespace GrpcReports;

public partial class ReportGrpcService : Reports.ReportsBase
{
    private readonly ReportsAppSettingsModel settings;
    private readonly IProductService productService;
    private readonly IOutstandingService outstandingService;
    private readonly IWinLossDetailService winLossDetailService;

    public ReportGrpcService(
        IOptions<ReportsAppSettingsModel> settings,
        IProductService productService,
        IOutstandingService outstandingService,
        IWinLossDetailService winLossDetailService)
    {
        this.settings = settings.Value;
        this.productService = productService;
        this.outstandingService = outstandingService;
        this.winLossDetailService = winLossDetailService;
    }
}