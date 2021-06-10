using InfluxDbData;
using BudgetManager.ManagerWeb.Extensions;
using BudgetManager.ManagerWeb.Models.SettingModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BudgetManager.ManagerWeb
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
            services.AddCors();
            IConfigurationSection connectinoStringSection = Configuration.GetSection(nameof(DbSetting));

            Influxdb influxdbConfig = new Influxdb();
            Configuration.Bind(nameof(Influxdb), influxdbConfig);

            // configure basic authentication 
            services
                .AddAuthentication(options => options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => options.LoginPath = "/User/Authenticate")
                .AddGoogle(options =>
                {
                    options.ClientId = "[MyGoogleClientId]";
                    options.ClientSecret = "[MyGoogleSecretKey]";
                }); ;

            services.Configure<DbSetting>(connectinoStringSection);
            services.AddHttpContextAccessor();

            services.ConfigureDataContext(Configuration.GetSection($"{nameof(DbSetting)}:ConnectionString").Value);
            services.ConfigureIoCRepositories();
            services.ConfigureInfluxRepositories();
            services.RegisterServices();
            services.AddTransient<IInfluxContext>(_ => new InfluxContext(influxdbConfig.Url, influxdbConfig.Token));

            services.AddSwaggerGen();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
