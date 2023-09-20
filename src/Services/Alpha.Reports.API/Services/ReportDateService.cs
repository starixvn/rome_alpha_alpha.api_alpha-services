using Alpha.Reports.API.Criteria;
using Alpha.Reports.API.Models;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.Reports.API.Services;

public interface IReportDateService
{
    DateTime GetBalanceUpDay();

    DateTime GetMoveBetsDay();
}

public class ReportDateService : IReportDateService
{
    private const string BalanceUpDayAndMoveBetsDayCacheKey = "Alpha.Reports.BalanceUpDayAndMoveBetsDay";

    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;

    public ReportDateService(IDynamicRepository dynamicRepository, ICacheService cacheService)
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
    }

    public static ReportDateService Instance { get; set; }

    public DateTime GetBalanceUpDay() => GetBalanceUpDayAndMoveBetsDay().BalanceUpDay;

    public DateTime GetMoveBetsDay() => GetBalanceUpDayAndMoveBetsDay().MoveBetsDay;

    public BalanceUpDayAndMoveBetsDayModel GetBalanceUpDayAndMoveBetsDay()
    {
        var balanceUpDayAndMoveBetsDay = cacheService.GetOrSet(
            BalanceUpDayAndMoveBetsDayCacheKey,
            () =>
            {
                var reportDates = dynamicRepository.Get<BalanceUpDayAndMoveBetsDayModel>(
                    new GetBalanceUpDayAndMoveBetsDayCriteria());

                return reportDates;
            },
            new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(2)));

        return balanceUpDayAndMoveBetsDay;
    }


}