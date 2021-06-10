using BudgetManager.FinanceDataMining.StockApi;
using BudgetManager.FinancialDataProvider.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;

namespace BudgetManager.FinancialDataProvider
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<StockOptions>(Configuration.GetSection(nameof(StockOptions)));
            string url = Configuration.GetSection($"{nameof(StockOptions)}:Uri").Value;
            string token = Configuration.GetSection($"{nameof(StockOptions)}:Token").Value;
            services.AddTransient(_ => new StockSetting { FinhubApiUrlBase = url, Token = token });
            services.AddTransient(_ => new HttpClient());
            services.AddTransient<IFinnhubStockApi, FinnhubStockApi>();

            services.AddSwaggerGen();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
