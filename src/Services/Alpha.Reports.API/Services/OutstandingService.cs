using Alpha.Reports.API.Models;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Criteria;
using Fanex.Data.Repository;
using Alpha.PartialUM.Services;
using Alpha.Utilities.Models;

namespace Alpha.Reports.API.Services;

public interface IOutstandingService
{
    Task<IEnumerable<OutstandingModel>> GetReport(GetOutstandingReportParam param);
}

public class OutstandingService : IOutstandingService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly IAutoFollowBetService autoFollowBetService;
    private readonly IPartialUMService partialUMService;

    public OutstandingService(
        IDynamicRepository dynamicRepository,
        IAutoFollowBetService autoFollowBetService,
        IPartialUMService partialUMService)
    {
        this.dynamicRepository = dynamicRepository;
        this.autoFollowBetService = autoFollowBetService;
        this.partialUMService = partialUMService;
    }

    public async Task<IEnumerable<OutstandingModel>> GetReport(GetOutstandingReportParam param)
    {
        if (!param.IsValid())
        {
            return Enumerable.Empty<OutstandingModel>();
        }

        var criteria = new GetOutstandingCriteria(param.CustLevel, param.CustId, param.ProductIds);
        partialUMService.ThrowIfUM(criteria.GetSettingKey());

        return param.AutoFollowBetIsDummyAgent
            ? await GetReport(param.ProductIds, await autoFollowBetService.GetFollowBetAccounts(param.CustId))
            : await dynamicRepository.FetchAsync<OutstandingModel>(criteria);
    }

    private async Task<IEnumerable<OutstandingModel>> GetReport(
        string productIds,
        IEnumerable<Tuple<int, IEnumerable<FollowBetAccountModel>>> followBetAccounts)
    {
        var outstandingModels = new List<OutstandingModel>();

        foreach (var item in followBetAccounts)
        {
            var outstanding = await GetReport(productIds, item);

            outstandingModels.AddRange(outstanding);
        }

        return outstandingModels;
    }

    private async Task<IEnumerable<OutstandingModel>> GetReport(
        string productIds,
        Tuple<int, IEnumerable<FollowBetAccountModel>> followBetAccount)
    {
        var agentId = followBetAccount.Item1;
        var followBetAccountEntitys = followBetAccount.Item2;
        var criteria = new GetOutstandingCriteria(UserLevelModel.Agent, agentId, productIds);
        var outstandingModels = await dynamicRepository.FetchAsync<OutstandingModel>(criteria);

        return outstandingModels.Join(
            followBetAccountEntitys,
            outstanding => outstanding.CustId,
            account => account.CustId,
            (outstanding, account) =>
            {
                outstanding.CustId = account.DummyCustId;
                outstanding.CustName = account.DummyUsername;

                return outstanding;
            });
    }
}