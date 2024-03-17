using Autofac;
using BudgetManager.Api.Middlewares;
using BudgetManager.Api.Models;
using BudgetManager.Api.Services;
using BudgetManager.Api.Services.SettingModels;
using BudgetManager.Core.SystemWrappers;
using BudgetManager.Data;
using BudgetManager.Domain.MessagingContracts;
using BudgetManager.InfluxDbData;
using BudgetManager.Repository.Extensions;
using BudgetManager.Services.Contracts;
using BudgetManager.Services.Extensions;
using BudgetManager.WebCore;
//using MassTransit;
using BudgetManager.WebCore.Extensions;
using MassTransit.RabbitMqTransport.Topology;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System.Text.Json.Serialization;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

namespace BudgetManager.Api
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

            services.AddControllers();
            services.Configure<AuthApiSetting>(Configuration.GetSection("AuthApi"));
            services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddHttpClient();

            RabbitMqConfig rabbitSetting = this.Configuration.GetSection("Rabbit").Get<RabbitMqConfig>();
            rabbitSetting.EndpointsConfiguration = c => c.Publish<TickerRequest>(x => x.ExchangeType = ExchangeType.Direct);
            services.AddMassTransitWithRabbitMq(rabbitSetting);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44386", "https://localhost:5001")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                    });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BudgetManager.Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                   {
                     new OpenApiSecurityScheme
                     {
                       Reference = new OpenApiReference
                       {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                       }
                      },
                      new string[] { }
                    }
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.Register<IHttpContextAccessor>(a => new HttpContextAccessor());
            DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBuilder.UseSqlServer(Configuration.GetSection($"{nameof(DbSetting)}:ConnectionString").Value);
            builder.Register(_ => new DataContext(optionsBuilder.Options));
            builder.RegisterType<UserDataProviderService>().As<IUserDataProviderService>();
            builder.RegisterType<DateTimeWrap>().As<IDateTime>();
            builder.RegisterRepositories();
            builder.RegisterServices();

            InfluxSetting influxSetting = this.Configuration.GetSection("Influxdb").Get<InfluxSetting>();
            builder.RegisterInstance(new InfluxContext(influxSetting.Url, influxSetting.Token, influxSetting.OrganizationId)).As<IInfluxContext>();
            builder.RegisterType<CryptoData>();
            builder.RegisterType<ForexData>();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
            builder.RegisterModelMapping();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BudgetManager.Api v1"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
