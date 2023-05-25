using Alpha.BetLists.API.Criteria;
using Alpha.IPInfo.DataAccess;
using Alpha.IPInfo.Models;
using Fanex.Data.Repository;

namespace Alpha.BetLists.API.Repositories
{
    public class IPInfoDataAccess : IIPInfoDataAccess
    {
        private readonly IDynamicRepository dynamicRepository;

        public IPInfoDataAccess(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository;
        }

        public async Task<IEnumerable<IPV4Information>> GetIPV4InfoByTopCountries(string countries)
        {
            GetIPRangesByCountryCriteria criteria = new GetIPRangesByCountryCriteria(false, countries);

            return await dynamicRepository.FetchAsync<IPV4Information>(criteria);
        }

        public async Task<IEnumerable<IPV4Information>> GetIPV4Info(string ipList)
        {
            GetIPRangesInfoByIPList criteria = new GetIPRangesInfoByIPList(ipList);

            return await dynamicRepository.FetchAsync<IPV4Information>(criteria);
        }

        public async Task<IEnumerable<IPV6Information>> GetIPV6Info(string ipList)
        {
            GetIPRangesInfoByIPV6List criteria = new GetIPRangesInfoByIPV6List(ipList);

            return await dynamicRepository.FetchAsync<IPV6Information>(criteria);
        }
    }
}