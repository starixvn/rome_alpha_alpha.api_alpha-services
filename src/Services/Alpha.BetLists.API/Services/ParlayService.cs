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

    public class ParlayService : IParlayService
    {
        private readonly IReportDateService reportDateService;
        private readonly IDynamicRepository dynamicRepository;

        public ParlayService(
            IReportDateService reportDateService,
            IDynamicRepository dynamicRepository)
        {
            this.reportDateService = reportDateService;
            this.dynamicRepository = dynamicRepository;
        }

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

            var comboParlayDetailTuple = await dynamicRepository.FetchMultipleAsync<ComboParlayDetail, MatchDetail>(criteria);

            return new ComboParlayDetailData(comboParlayDetailTuple.Item1, comboParlayDetailTuple.Item2, criteria.Outstanding, criteria.IsParlayBoost);
        }
    }
}