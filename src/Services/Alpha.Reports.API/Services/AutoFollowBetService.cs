using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Services;

public interface IAutoFollowBetService
{
    Task<IEnumerable<Tuple<int, IEnumerable<FollowBetAccountModel>>>> GetFollowBetAccounts(int agentId);
}

public class AutoFollowBetService : IAutoFollowBetService
{
    private readonly IDynamicRepository dynamicRepository;

    public AutoFollowBetService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task<IEnumerable<Tuple<int, IEnumerable<FollowBetAccountModel>>>> GetFollowBetAccounts(int agentId)
    {
        var criteria = new GetFollowBetAccountsCriteria(agentId);
        var followBetAccounts = await dynamicRepository.FetchAsync<FollowBetAccountModel>(criteria);

        return followBetAccounts
            .GroupBy(f => f.AgentId, (key, accounts) => Tuple.Create(key, accounts));
    }
}