using InfluxDbData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerWeb.Services
{
    public class ForexService : IForexService
    {
        private const string bucketForex = "Forex";
        private const string organizationId = "8f46f33452affe4a";
        private readonly IRepository<ForexData> forexRepository;

        public ForexService(IRepository<ForexData> forexRepository)
        {
            this.forexRepository = forexRepository;
        }

        public async Task<ForexData> GetCurrentExchangeRate(string fromSymbol, string toSymbol)
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(organizationId, bucketForex);
            List<ForexData> data = await this.forexRepository.GetLastWrittenRecordsTime(dataSourceIdentification);
            return data.SingleOrDefault(a => string.Equals(a.BaseCurrency, fromSymbol, System.StringComparison.OrdinalIgnoreCase)
                && string.Equals(a.Currency, toSymbol, System.StringComparison.OrdinalIgnoreCase));
        }
    }
}
