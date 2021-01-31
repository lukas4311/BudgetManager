using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace TestingConsole
{
    public class ConfigManager
    {
        public InfluxConfig GetSecretToken()
        {
            InfluxConfig influxConfig = new InfluxConfig();
            IConfigurationRoot configuration = this.GetConfigurationRoot();
            configuration.GetSection("Influxdb").Bind(influxConfig);

            return influxConfig;
        }

        public string GetConnectionString()
        {
            IConfigurationRoot configuration = this.GetConfigurationRoot();
            return configuration.GetConnectionString("DefaultConnection");
        }

        private IConfigurationRoot GetConfigurationRoot()
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Single(o => o.EntryPoint != null);
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            return configurationBuilder.AddUserSecrets(assembly, optional: false).Build();
        }
    }
}
