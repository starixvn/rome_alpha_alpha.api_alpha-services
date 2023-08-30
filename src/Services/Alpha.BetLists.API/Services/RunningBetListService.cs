
using Alpha.PartialUM.Services;
using Fanex.Data.Repository;
using Alpha.BetLists.API.Params;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Criteria;
using Alpha.Utilities.Extensions;
using Fanex.BetList.Core.Entities;

namespace Alpha.BetLists.API.Services;

public interface IRunningBetListService
{
    Task<BetListData> GetBetList(GetRunningBetListParam param);
}

public class RunningBetListService : IRunningBetListService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly IPartialUMService partialUMService;

    public RunningBetListService(
        IDynamicRepository dynamicRepository,
        IPartialUMService partialUMService)
    {
        this.dynamicRepository = dynamicRepository;
        this.partialUMService = partialUMService;
    }

    public async Task<BetListData> GetBetList(GetRunningBetListParam param)
    {
        var criteria = new GetBetListRunningByDateCriteria(
            param.CustId,
            param.ProductIds.JoinWithComma());

        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        var betListDataTuple = await dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(criteria);

        return new BetListData(betListDataTuple.Item1, betListDataTuple.Item2.ToList<ITicketData>());
    }
}