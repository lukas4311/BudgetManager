using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using ManagerWeb.Extensions;
using ManagerWeb.Models.SettingModels;
using ManagerWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;

namespace ManagerWeb
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

            // configure basic authentication 
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.Configure<DbSetting>(connectinoStringSection);
            services.ConfigureDataContext(Configuration.GetSection($"{nameof(DbSetting)}:ConnectionString").Value);
            services.AddTransient<IPaymentCategoryRepository, PaymentCategoryRepository>();
            services.AddTransient<IPaymentTypeRepository, PaymentTypeRepository>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
            services.AddTransient<IBankAccountRepository, BankAccountRepository>();
            services.AddTransient<IUserIdentityRepository, UserIdentityRepository>();
            services.AddTransient<IUserService, UserService>();

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
