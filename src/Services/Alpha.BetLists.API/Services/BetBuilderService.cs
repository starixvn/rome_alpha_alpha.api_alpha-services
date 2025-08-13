namespace Alpha.BetLists.API.Services;
using Alpha.BetLists.API.Criteria;
using Alpha.BetLists.API.Models.TicketDetail;
using Alpha.BetLists.API.Resources;
using Fanex.Data.Repository;
using Fanex.RemotingRef;
using System.Globalization;

public interface IBetBuilderService
{
    Task<IEnumerable<BetBuilderDetailModel>> GetBetList(long custId, long transId);
}

public class BetBuilderService(IDynamicRepository repository, IRemotingRefService remotingRefService) : IBetBuilderService
{
    private readonly IDynamicRepository repository = repository;
    private readonly IRemotingRefService remotingRefService = remotingRefService;

    public async Task<IEnumerable<BetBuilderDetailModel>> GetBetList(long custId, long transId)
    {
        var criteria = new GetBetBuilderDetailCriteria(custId, transId);
        var details = await repository.FetchAsync<BetBuilderDetailModel>(criteria);

        foreach (var detail in details)
        {
            var names = remotingRefService.GetBetBuilderSelectionNames(
                new[] { detail.SelectionType },
                CultureInfo.CurrentCulture.Name);

            detail.SelectionName = names is { Length: 1 }
                ? names[0]
                : detail.SelectionType.ToString();

            if (!string.IsNullOrWhiteSpace(detail.Result))
            {
                detail.Result = BetListResources.ResourceManager
                    .GetString(detail.Result.ToLowerInvariant()) ?? detail.Result;
            }
        }

        return details;
    }
}
