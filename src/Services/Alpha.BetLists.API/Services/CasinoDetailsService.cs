using Alpha.BetLists.API.Criteria;
using Alpha.BetLists.API.Models;
using Alpha.BetLists.API.Params;
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Services
{
    public interface ICasinoDetailsService
    {
        PagedData<TicketDto> GetCasinoDetails(CasinoDetailsParam param);
    }

    public class CasinoDetailsService : ICasinoDetailsService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IReportDateService reportDateService;

        public CasinoDetailsService(IDynamicRepository dynamicRepository, IReportDateService reportDateService)
        {
            this.dynamicRepository = dynamicRepository;
            this.reportDateService = reportDateService;
        }

        public PagedData<TicketDto> GetCasinoDetails(CasinoDetailsParam param)
        {
            var criteria = new GetCasinoDetailsCriteria(
                param.RefNo,
                param.CustId,
                param.TransType,
                param.ProductId,
                param.PageIndex,
                param.PageSize,
                reportDateService.GetMoveBetsDay(),
                reportDateService.GetBalanceUpDay());

            var tickets = dynamicRepository.Fetch<TicketDto>(criteria);

            return new PagedData<TicketDto>(tickets, criteria.PageIndex, criteria.PageSize, criteria.TotalRows);
        }
    }
}