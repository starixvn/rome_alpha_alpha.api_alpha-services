
using Alpha.PartialUM.Services;
using Fanex.Data.Repository;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Criteria;
using Alpha.Utilities.Extensions;
using Fanex.BetList.Core.Entities;
using Alpha.BetLists.API.Resources;
using Alpha.Shared.Services.Services;

namespace Alpha.BetLists.API.Services;

public interface IWinLossBetListService
{
    Task<BetListData> GetBetList(GetWinLossBetListParam param);
}

public class WinLossBetListService : IWinLossBetListService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly IPartialUMService partialUMService;
    private readonly IReportDateService reportDateService;
    private readonly IProductDetailService productDetailService;

    public WinLossBetListService(
        IDynamicRepository dynamicRepository,
        IPartialUMService partialUMService,
        IReportDateService reportDateService,
        IProductDetailService productDetailService)
    {
        this.dynamicRepository = dynamicRepository;
        this.partialUMService = partialUMService;
        this.reportDateService = reportDateService;
        this.productDetailService = productDetailService;
    }

    public async Task<BetListData> GetBetList(GetWinLossBetListParam param)
    {
        var fromDate = param.FromDate;
        var betListDataTuple = await GetBetList(param, fromDate);

        return new BetListData(betListDataTuple.Item1, betListDataTuple.Item2.ToList<ITicketData>(), betListDataTuple.Item3);
    }

    private async Task<Tuple<IEnumerable<TicketDto>, IEnumerable<TicketData>, BetlistOutputParams>> GetBetList(GetWinLossBetListParam param, DateTime fromDate)
    {
        var productDetailsXML = productDetailService.BuildProductDetailsXML(param.ProductIds);

        var criteria = new GetBetListWinLossByDateCriteria(
            param.CustId,
            param.ProductIds.JoinWithComma(),
            fromDate,
            param.ToDate,
            productDetailsXML,
            reportDateService.GetMoveBetsDay(),
            param.MaxTotalRow);

        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        var (tickets, details) = await dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(criteria);
        var errFormat =
            string.Format(BetListResources.ExceededMaxTotalRowWithExcelFunc, criteria.MaxTotalRow, param.MaxTotalRow);

        return new Tuple<IEnumerable<TicketDto>, IEnumerable<TicketData>, BetlistOutputParams>(
            tickets,
            details,
            new BetlistOutputParams(criteria.ErrCode, criteria.ErrCode != 0 ?
                errFormat : criteria.ErrMsg, criteria.MaxTotalRow));
    }
}