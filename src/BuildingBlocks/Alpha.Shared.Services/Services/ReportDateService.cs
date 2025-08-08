using Alpha.Shared.Services.Criteria;
using Alpha.Shared.Services.Models;
using Fanex.Caching;
using Fanex.Data.Repository;

namespace Alpha.Shared.Services.Services;

/// <summary>
/// Interface for report date service operations
/// </summary>
public interface IReportDateService
{
    DateTime GetBalanceUpDay();
    DateTime GetMoveBetsDay();
    BalanceUpDayAndMoveBetsDayModel GetBalanceUpDayAndMoveBetsDay();
}

/// <summary>
/// Shared implementation of report date service
/// Provides caching and database access for balance up day and move bets day information
/// Used across Alpha.Reports.API and Alpha.BetLists.API
/// </summary>
public class ReportDateService : IReportDateService
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly ICacheService cacheService;
    private readonly string cacheKeyPrefix;

    public ReportDateService(IDynamicRepository dynamicRepository, ICacheService cacheService, string cacheKeyPrefix = "Alpha.Shared")
    {
        this.dynamicRepository = dynamicRepository;
        this.cacheService = cacheService;
        this.cacheKeyPrefix = cacheKeyPrefix;
    }

    public static ReportDateService? Instance { get; set; }

    public DateTime GetBalanceUpDay() => GetBalanceUpDayAndMoveBetsDay().BalanceUpDay;

    public DateTime GetMoveBetsDay() => GetBalanceUpDayAndMoveBetsDay().MoveBetsDay;

    public BalanceUpDayAndMoveBetsDayModel GetBalanceUpDayAndMoveBetsDay()
    {
        var cacheKey = $"{cacheKeyPrefix}.BalanceUpDayAndMoveBetsDay";
        
        var balanceUpDayAndMoveBetsDay = cacheService.GetOrSet(
            cacheKey,
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
