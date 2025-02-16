using BudgetManager.InfluxDbData;
using BudgetManager.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BudgetManager.Services
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class ForexService : IForexService
    {
        private const string bucketForex = "Forex";
        private const string bucketForexV2 = "ForexV2";
        private readonly IRepository<ForexData> forexRepository;
        private readonly IRepository<ForexDataV2> forexRepositoryV2;
        private readonly IInfluxContext influxContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CryptoService"/> class.
        /// </summary>
        /// <param name="forexRepository">Forex repository.</param>
        /// <param name="forexRepositoryV2">Forex repository v2.</param>
        /// <param name="influxContext">The InfluxDB context.</param>
        public ForexService(IRepository<ForexData> forexRepository, IRepository<ForexDataV2> forexRepositoryV2, IInfluxContext influxContext)
        {
            this.forexRepository = forexRepository;
            this.forexRepositoryV2 = forexRepositoryV2;
            this.influxContext = influxContext;
        }

        /// <inheritdoc/>
        public async Task<double> GetCurrentExchangeRate(string fromSymbol, string toSymbol)
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(influxContext.OrganizationId, bucketForex);
            IEnumerable<ForexData> data = await forexRepository.GetLastWrittenRecordsTime(dataSourceIdentification);
            return data.SingleOrDefault(a => string.Equals(a.BaseCurrency, fromSymbol, System.StringComparison.OrdinalIgnoreCase)
                && string.Equals(a.Currency, toSymbol, System.StringComparison.OrdinalIgnoreCase))?.Price ?? 0;
        }

        /// <inheritdoc/>
        public async Task<double> GetExchangeRate(string fromSymbol, string toSymbol)
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(influxContext.OrganizationId, bucketForexV2);
            IEnumerable<ForexDataV2> data = await forexRepositoryV2.GetLastWrittenRecordsTime(dataSourceIdentification);
            return data.SingleOrDefault(a => string.Equals(a.Pair, $"{fromSymbol}-{toSymbol}", System.StringComparison.OrdinalIgnoreCase))?.Price ?? 0;
        }

        /// <inheritdoc/>
        public async Task<double> GetExchangeRate(string fromSymbol, string toSymbol, DateTime atDate)
        {
            DataSourceIdentification dataSourceIdentification = new DataSourceIdentification(influxContext.OrganizationId, bucketForexV2);
            string pair = $"{fromSymbol}-{toSymbol}";
            IEnumerable<ForexDataV2> data = await forexRepositoryV2.GetAllData(dataSourceIdentification,
                new DateTimeRange { From = atDate.AddDays(-5), To = atDate.AddDays(1) }, new() { { "pair", pair } }).ConfigureAwait(false);
            return data.LastOrDefault()?.Price ?? 0;
        }
    }
}
