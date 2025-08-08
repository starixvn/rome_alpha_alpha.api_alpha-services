using Alpha.PartialUM.Services;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.DataAccesses;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Reports.API.Params;
using Alpha.Shared.Services.Services;

namespace Alpha.Reports.API.Services;

public interface IWinLossDetailService
{
    Task<ReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailReportParam param);
    Task<ReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailReportParam param);
    Task<ReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetWinLossDetailReportParam param);
}

public class WinLossDetailService : IWinLossDetailService
{
    private readonly IPartialUMService partialUMService;
    private readonly IProductService productService;
    private readonly IReportDateService reportDateService;
    private readonly IProductDetailService productDetailService;
    private readonly IWinLossDetailDataAccess winLossDetailDataAccess;

    public WinLossDetailService(
        IPartialUMService partialUMService,
        IProductService productService,
        IProductDetailService productDetailService,
        IReportDateService reportDateService,
        IWinLossDetailDataAccess winLossDetailDataAccess)
    {
        this.partialUMService = partialUMService;
        this.productService = productService;
        this.productDetailService = productDetailService;
        this.reportDateService = reportDateService;
        this.winLossDetailDataAccess = winLossDetailDataAccess;
    }

    public async Task<ReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailReportParam param)
    {
        if (!param.IsValid())
        {
            return new ReportDataModel<SuperWinLossDetailModel>(Enumerable.Empty<SuperWinLossDetailModel>(), new SuperWinLossDetailModel());
        }

        var balanceUpDay = reportDateService.GetBalanceUpDay();
        var moveBetsDay = reportDateService.GetMoveBetsDay();
        var products = productService.FilterProductIdsByCustomer(param.ProductIds, param.MinDate);
        products = productService.FilterProductIdsByGroup(products, param.ProductGroup);

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i));
        var productDetails = productDetailService.GetProductDetails(param.Settings).Select(r => r.ResourceId);
        var productDetailsXML = productDetailService.BuildProductDetailsXML(products, productDetails, productOfferProductDetail);

        var criteria = new GetWinLossDetailCriteria(
            param.CustId,
            balanceUpDay,
            moveBetsDay,
            products,
            param.CustLevel,
            productDetailsXML,
            param.FromDate,
            param.ToDate,
            param.Level);

        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        var winLossDetailData = await winLossDetailDataAccess.GetSuperWinLossDetail(criteria);

        return winLossDetailData;
    }

    public async Task<ReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailReportParam param)
    {
        if (!param.IsValid())
        {
            return new ReportDataModel<MasterWinLossDetailModel>(Enumerable.Empty<MasterWinLossDetailModel>(), new MasterWinLossDetailModel());
        }

        var balanceUpDay = reportDateService.GetBalanceUpDay();
        var moveBetsDay = reportDateService.GetMoveBetsDay();
        var products = productService.FilterProductIdsByCustomer(param.ProductIds, param.MinDate);
        products = productService.FilterProductIdsByGroup(products, param.ProductGroup);

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i));
        var productDetails = productDetailService.GetProductDetails(param.Settings).Select(r => r.ResourceId);
        var productDetailsXML = productDetailService.BuildProductDetailsXML(products, productDetails, productOfferProductDetail);

        var criteria = new GetWinLossDetailCriteria(
            param.CustId,
            balanceUpDay,
            moveBetsDay,
            products,
            param.CustLevel,
            productDetailsXML,
            param.FromDate,
            param.ToDate,
            param.Level);

        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        var winLossDetailData = await winLossDetailDataAccess.GetMasterWinLossDetail(criteria);

        return winLossDetailData;
    }

    public async Task<ReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetWinLossDetailReportParam param)
    {
        if (!param.IsValid())
        {
            return new ReportDataModel<AgentWinLossDetailModel>(Enumerable.Empty<AgentWinLossDetailModel>(), new AgentWinLossDetailModel());
        }

        var balanceUpDay = reportDateService.GetBalanceUpDay();
        var moveBetsDay = reportDateService.GetMoveBetsDay();

        var products = productService.FilterProductIdsByCustomer(param.ProductIds, param.MinDate);
        products = productService.FilterProductIdsByGroup(products, param.ProductGroup);

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i));

        var productDetails = productDetailService.GetProductDetails(param.Settings).Select(r => r.ResourceId);

        var productDetailsXML = productDetailService.BuildProductDetailsXML(products, productDetails, productOfferProductDetail);

        var criteria = new GetMemberWinLossDetailCriteria(
            param.CustId,
            balanceUpDay,
            moveBetsDay,
            products,
            param.CustLevel,
            productDetailsXML,
            param.FromDate,
            param.ToDate);

        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        var winLossDetailData = await winLossDetailDataAccess.GetAgentWinLossDetail(criteria);

        return winLossDetailData;
    }
}