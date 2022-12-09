
using Alpha.PartialUM.Services;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.DataAccesses;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Reports.API.Params;

namespace Alpha.Reports.API.Services;

public interface IWinLossDetailService
{
    Task<PagedReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailReportParam param);
    Task<PagedReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailReportParam param);
    Task<PagedReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetWinLossDetailReportParam param);
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

    public async Task<PagedReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailReportParam param)
    {
        if (!param.IsValid())
        {
            return new PagedReportDataModel<SuperWinLossDetailModel>(
                Enumerable.Empty<SuperWinLossDetailModel>(),
                param.PageIndex,
                param.PageSize,
                0,
                new SuperWinLossDetailModel());
        }

        var balanceUpDay = reportDateService.GetBalanceUpDay();
        var moveBetsDay = reportDateService.GetMoveBetsDay();
        var products = await productService.FilterProductIdsByCustomer(
            param.ViewerId,
            param.Site,
            param.CurrencyId,
            param.Language,
            param.ProductIds,
            isEnableSabaPromotion: param.IsEnableSabaPromotion);

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i));
        var productDetails = (await productDetailService.GetProductDetails(param.ViewerId, param.Site, param.CurrencyId, param.Language, param.Settings)).Select(r => r.ResourceId);
        var productDetailsXML = productDetailService.BuildProductDetailsXML(products, productDetails, productOfferProductDetail);

        var criteria = new GetWinLossDetailCriteria(
            param.CustId,
            param.PageIndex,
            param.PageSize,
            balanceUpDay,
            moveBetsDay,
            products,
            param.ViewerLevel,
            productDetailsXML);

        partialUMService.ThrowIfUM(criteria.GetSettingKey());

        var winLossDetailData = await winLossDetailDataAccess.GetSuperWinLossDetail(criteria);

        return winLossDetailData;
    }

    public async Task<PagedReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailReportParam param)
    {
        if (!param.IsValid())
        {
            return new PagedReportDataModel<MasterWinLossDetailModel>(
                Enumerable.Empty<MasterWinLossDetailModel>(),
                param.PageIndex,
                param.PageSize,
                0,
                new MasterWinLossDetailModel());
        }

        var balanceUpDay = reportDateService.GetBalanceUpDay();
        var moveBetsDay = reportDateService.GetMoveBetsDay();
        var products = await productService.FilterProductIdsByCustomer(
            param.ViewerId,
            param.Site,
            param.CurrencyId,
            param.Language,
            param.ProductIds,
            isEnableSabaPromotion: param.IsEnableSabaPromotion);

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i));
        var productDetails = (await productDetailService.GetProductDetails(param.ViewerId, param.Site, param.CurrencyId, param.Language, param.Settings)).Select(r => r.ResourceId);
        var productDetailsXML = productDetailService.BuildProductDetailsXML(products, productDetails, productOfferProductDetail);

        var criteria = new GetWinLossDetailCriteria(
            param.CustId,
            param.PageIndex,
            param.PageSize,
            balanceUpDay,
            moveBetsDay,
            products,
            param.ViewerLevel,
            productDetailsXML);

        partialUMService.ThrowIfUM(criteria.GetSettingKey());

        var winLossDetailData = await winLossDetailDataAccess.GetMasterWinLossDetail(criteria);

        return winLossDetailData;
    }

    public async Task<PagedReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetWinLossDetailReportParam param)
    {
        if (!param.IsValid())
        {
            return new PagedReportDataModel<AgentWinLossDetailModel>(
                Enumerable.Empty<AgentWinLossDetailModel>(),
                param.PageIndex,
                param.PageSize,
                0,
                new AgentWinLossDetailModel());
        }

        var balanceUpDay = reportDateService.GetBalanceUpDay();
        var moveBetsDay = reportDateService.GetMoveBetsDay();
        var products = await productService.FilterProductIdsByCustomer(
            param.ViewerId,
            param.Site,
            param.CurrencyId,
            param.Language,
            param.ProductIds,
            param.MinDate,
            isEnableSabaPromotion: param.IsEnableSabaPromotion);

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => int.Parse(i));
        var productDetails = (await productDetailService.GetProductDetails(param.ViewerId, param.Site, param.CurrencyId, param.Language, param.Settings)).Select(r => r.ResourceId);
        var productDetailsXML = productDetailService.BuildProductDetailsXML(products, productDetails, productOfferProductDetail);

        var criteria = new GetWinLossDetailCriteria(
            param.CustId,
            param.PageIndex,
            param.PageSize,
            balanceUpDay,
            moveBetsDay,
            products,
            param.ViewerLevel,
            productDetailsXML);

        partialUMService.ThrowIfUM(criteria.GetSettingKey());

        var winLossDetailData = await winLossDetailDataAccess.GetAgentWinLossDetail(criteria);

        return winLossDetailData;
    }
}