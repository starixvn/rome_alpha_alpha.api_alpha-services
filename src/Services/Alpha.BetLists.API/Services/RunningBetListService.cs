
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

public class RunningBetListService(IDynamicRepository dynamicRepository, IPartialUMService partialUMService) : IRunningBetListService
{
    private readonly IDynamicRepository dynamicRepository = dynamicRepository;
    private readonly IPartialUMService partialUMService = partialUMService;

    public async Task<BetListData> GetBetList(GetRunningBetListParam param)
    {
        var criteria = new GetBetListRunningByDateCriteria(
            param.CustId,
            param.ProductIds.JoinWithComma());

        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        var (tickets, details) = await dynamicRepository.FetchMultipleAsync<TicketDto, TicketData>(criteria);

        return new BetListData(tickets, [.. details]);
    }
}