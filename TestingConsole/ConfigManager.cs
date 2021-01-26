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

            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Single(o => o.EntryPoint != null);
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            IConfigurationRoot configuration = configurationBuilder.AddUserSecrets(assembly, optional: false).Build();
            configuration.GetSection("Influxdb").Bind(influxConfig);

            return influxConfig;
        }
    }
}
