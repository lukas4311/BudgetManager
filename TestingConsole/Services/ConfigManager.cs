using BudgetManager.TestingConsole.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;

namespace BudgetManager.TestingConsole
{
    public class ConfigManager
    {
        private const string QuandlSettingSectionKey = "QuandlSetting";
        private const string TwelveDataKeySection = "TwelveDataApi";
        private const string TwelveDataKey = "ApiKey";

        public InfluxConfig GetSecretToken()
        {
            InfluxConfig influxConfig = new InfluxConfig();
            IConfigurationRoot configuration = this.GetConfigurationRoot();
            configuration.GetSection("Influxdb").Bind(influxConfig);

            return influxConfig;
        }

        public QuandlSetting GetQuandlSetting()
        {
            QuandlSetting quandlSetting = new QuandlSetting();
            IConfiguration configuration = this.GetConfigurationRoot();
            configuration.GetSection(QuandlSettingSectionKey).Bind(quandlSetting);

            return quandlSetting;
        }

        public string GetConnectionString()
        {
            IConfigurationRoot configuration = this.GetConfigurationRoot();
            return configuration.GetConnectionString("DefaultConnection");
        }

        public string GetTwelveDataApiKey()
        {
            return this.GetConfigurationRoot().GetSection(TwelveDataKeySection).GetValue<string>(TwelveDataKey);
        }

        private IConfigurationRoot GetConfigurationRoot()
        {
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Single(o => o.EntryPoint != null);
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            return configurationBuilder.AddUserSecrets(assembly, optional: false).Build();
        }
    }
}
