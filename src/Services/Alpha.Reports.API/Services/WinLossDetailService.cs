
using Alpha.PartialUM.Services;
using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.DataAccesses;
using Alpha.Reports.API.Models;
using Alpha.Reports.API.Models.WinLossDetail;
using Alpha.Reports.API.Params;
using Alpha.Utilities.Extensions;
using Alpha.Utilities.Models;
using Fanex.Data.Repository;
using GrpcCustomer;

namespace Alpha.Reports.API.Services;

public interface IWinLossDetailService
{
    Task<ReportDataModel<SuperWinLossDetailModel>> GetSuperWinLossDetail(GetWinLossDetailReportParam param);
    Task<ReportDataModel<MasterWinLossDetailModel>> GetMasterWinLossDetail(GetWinLossDetailReportParam param);
    Task<ReportDataModel<AgentWinLossDetailModel>> GetAgentWinLossDetail(GetWinLossDetailReportParam param);
    Task<SummaryWinLossDetail> GetSummaryWinLossDetail(GetWinLossDetailSummaryParam param);
}

public class WinLossDetailService : IWinLossDetailService
{
    private readonly IPartialUMService partialUMService;
    private readonly IProductService productService;
    private readonly IReportDateService reportDateService;
    private readonly IProductDetailService productDetailService;
    private readonly IWinLossDetailDataAccess winLossDetailDataAccess;
    private readonly Customer.CustomerClient customerClient;

    public WinLossDetailService(
        IPartialUMService partialUMService,
        IProductService productService,
        IProductDetailService productDetailService,
        IReportDateService reportDateService,
        IWinLossDetailDataAccess winLossDetailDataAccess,
        Customer.CustomerClient customerClient)
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

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.ToListInt();
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

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.ToListInt();
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

        var productOfferProductDetail = param.Settings.ProductOfferProductDetail.ToListInt();

        var productDetails = productDetailService.GetProductDetails(param.Settings).Select(r => r.ResourceId);

        var productDetailsXML = productDetailService.BuildProductDetailsXML(products, productDetails, productOfferProductDetail);

        var criteria = new GetMemberWinLossDetailCriteria(
            param.CustId,
            balanceUpDay,
            moveBetsDay,
            products,
            productDetailsXML,
            param.FromDate,
            param.ToDate);

        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        var winLossDetailData = await winLossDetailDataAccess.GetAgentWinLossDetail(criteria);

        return winLossDetailData;
    }

    public async Task<SummaryWinLossDetail> GetSummaryWinLossDetail(GetWinLossDetailSummaryParam param)
    {
        if (!param.IsValid())
        {
            return null;
        }

        var productOfferProductDetail = new HashSet<int>(param.Settings.ProductOfferProductDetail.ToListInt());
        var reportDate = reportDateService.GetBalanceUpDayAndMoveBetsDay();
        var productIds = productService.FilterProductIdsByCustomer(param.ProductNames, param.Language, param.MinDate).ToList();
        var (productDetailIds, productDetailsXML) = productDetailService.BuildProductDetailsXML(productIds, param.ProductDetails, productOfferProductDetail, param.Language);

        ICriteria criteria = UserLevelModel.IsAgentLevel(param.CustLevel)
            ? new GetMemberWinLossDetailCriteria(
                param.CustId,
                reportDate.BalanceUpDay,
                reportDate.MoveBetsDay,
                productIds,
                productDetailsXML,
                param.FromDate,
                param.ToDate)
            : new GetWinLossDetailCriteria(
                param.CustId,
                reportDate.BalanceUpDay,
                reportDate.MoveBetsDay,
                productIds,
                param.CustLevel,
                productDetailsXML,
                param.FromDate,
                param.ToDate,
                param.Level);
        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.Site);
        SummaryWinLossDetail total;

        if (UserLevelModel.IsSuperLevel(param.CustLevel))
        {
            total = await winLossDetailDataAccess.GetSuperWinLossDetailSummary(criteria, param.Username);
        }
        else if (UserLevelModel.IsMasterLevel(param.CustLevel))
        {
            total = await winLossDetailDataAccess.GetMasterWinLossDetailSummary(criteria, param.Username);
        }
        else
        {
            total = await winLossDetailDataAccess.GetAgentWinLossDetailSummary(criteria, param.Username);
        }

        if (total != null)
        {
            total.ProductIds = productIds.JoinWithComma();
            total.ProductDetailIds = productDetailIds.JoinWithComma();
        }

        return total;
    }
}