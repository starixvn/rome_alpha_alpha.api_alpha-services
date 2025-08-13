using Alpha.BetLists.API.Criteria.Parlay;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Models.Parlay;
using Fanex.Data.Repository;
using Alpha.Shared.Services.Services;

namespace Alpha.BetLists.API.Services
{
    public interface IParlayService
    {
        Task<IEnumerable<TicketDto>> GetParlayBetList(long transId, long custId);

        Task<ComboParlayDetailData> GetComboParlayDetail(long refNo, int custId);
    }

    public class ParlayService(IReportDateService reportDateService, IDynamicRepository dynamicRepository) : IParlayService
    {
        private readonly IReportDateService reportDateService = reportDateService;
        private readonly IDynamicRepository dynamicRepository = dynamicRepository;

        public async Task<IEnumerable<TicketDto>> GetParlayBetList(long transId, long custId)
        {
            var tickets = await dynamicRepository.FetchAsync<TicketDto>(new GetParlayBetListCriteria(transId, custId));

            return tickets;
        }

        public async Task<ComboParlayDetailData> GetComboParlayDetail(long refNo, int custId)
        {
            var criteria = new GetComboParlayDetailCriteria(
                    refNo,
                    custId,
                    reportDateService.GetBalanceUpDay(),
                    reportDateService.GetMoveBetsDay());

            var (details, matchs) = await dynamicRepository.FetchMultipleAsync<ComboParlayDetail, MatchDetail>(criteria);

            return new ComboParlayDetailData(details, matchs, criteria.Outstanding, criteria.IsParlayBoost);
        }
    }
}