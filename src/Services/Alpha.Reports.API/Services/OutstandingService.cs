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
    private readonly IPartialUMService partialUMService;

    public OutstandingService(IDynamicRepository dynamicRepository, IPartialUMService partialUMService)
    {
        this.dynamicRepository = dynamicRepository;
        this.partialUMService = partialUMService;
    }

    public async Task<IEnumerable<OutstandingModel>> GetReport(GetOutstandingReportParam param)
    {
        if (!param.IsValid())
        {
            return Enumerable.Empty<OutstandingModel>();
        }

        CriteriaBase criteria = param.CustLevel == UserLevelModel.Agent ?        
            new GetMemberOutstandingCriteria(param.CustId, param.ProductIds) :
            new GetOutstandingCriteria(param.CustLevel, param.CustId, param.ProductIds, param.Level);
        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        return await dynamicRepository.FetchAsync<OutstandingModel>(criteria);
    }
}