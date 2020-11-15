using Microsoft.Extensions.Configuration;
using System.IO;

namespace FinanceDataMining
{
    public class SettingService
    {
        private const string ConfigFileName = "config.json";

        public void LoadConfig()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            string path = Path.Combine(Directory.GetCurrentDirectory(), ConfigFileName);
            configurationBuilder.AddJsonFile(path, false);
            configurationBuilder.AddUserSecrets(this.GetType().Assembly, optional: true);
            IConfigurationRoot root = configurationBuilder.Build();
            //_sqlConnection = root.GetConnectionString("DataConnection");
        }
    }
}
