namespace InfluxDbData
{
    public class DataSourceIdentification
    {
        public DataSourceIdentification(string organication, string bucket)
        {
            this.Organization = organication;
            this.Bucket = bucket;
        }

        public string Organization { get; }
        public string Bucket { get; }
    }
}
