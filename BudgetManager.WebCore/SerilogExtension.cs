using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace BudgetManager.WebCore
{
    public static class SerilogExtension
    {
        private static readonly string ElkSection = "Elk";
        private static readonly string ElasticUrl = "ElasticUrl";
        private static readonly string ElasticIndex = "ElasaticIndexKey";

        public static IHostBuilder ConfigureHostWithSerilogToElk(this IHostBuilder host)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .Build();

            ConfigureLogging(environment, configuration);

            return host.UseSerilog();
        }

        private static void ConfigureLogging(string environment, IConfigurationRoot configuration)
        {
            string elaticUrl = configuration.GetValue<string>($"{ElkSection}:{ElasticUrl}");
            string index = configuration.GetValue<string>($"{ElkSection}:{ElasticIndex}");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elaticUrl))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                    IndexFormat = $"{index.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                })
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
