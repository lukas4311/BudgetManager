using InfluxDB.Client;

namespace BudgetManager.InfluxDbData
{
    public class InfluxContext : IInfluxContext
    {
        public InfluxContext(string influxUrl, string influxToken, string organizationId)
        {
            this.Client = InfluxDBClientFactory.Create(influxUrl, influxToken.ToCharArray());
            OrganizationId = organizationId;
        }

        public InfluxDBClient Client { get; }

        public string OrganizationId { get; }
    }
}
