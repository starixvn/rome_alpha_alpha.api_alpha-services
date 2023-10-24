using Alpha.Reports.API.Models;
using Alpha.Reports.API.Params;
using Alpha.Reports.API.Criteria;
using Fanex.Data.Repository;
using Alpha.PartialUM.Services;

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

        var criteria = new GetOutstandingCriteria(param.CustLevel, param.CustId, param.ProductIds);
        partialUMService.ThrowIfUM(criteria.GetSettingKey(), param.SiteName);

        return await dynamicRepository.FetchAsync<OutstandingModel>(criteria);
    }
}