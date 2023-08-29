using Autofac.Extensions.DependencyInjection;
using Autofac;
using BudgetManager.FinancialApi.Models;
using BudgetManager.InfluxDbData;
using BudgetManager.Services.Extensions;
using System.Text.Json.Serialization;
using JsnOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;
using BudgetManager.FinancialApi.Endpoints;
using BudgetManager.Data;
using Microsoft.EntityFrameworkCore;
using BudgetManager.Repository.Extensions;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("https://localhost:44386", "https://localhost:5001")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

builder.Services.Configure<JsnOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
    optionsBuilder.UseSqlServer(configuration.GetSection($"{nameof(DbSetting)}:ConnectionString").Value);
    containerBuilder.Register(_ => new DataContext(optionsBuilder.Options));
    containerBuilder.RegisterRepositories();
    containerBuilder.RegisterModelMapping();

    InfluxSetting influxSetting = configuration.GetSection("Influxdb").Get<InfluxSetting>();
    containerBuilder.RegisterServices();
    containerBuilder.RegisterInstance(new InfluxContext(influxSetting.Url, influxSetting.Token)).As<IInfluxContext>();
    containerBuilder.RegisterType<CryptoData>();
    containerBuilder.RegisterType<ForexData>();
    containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.RegisterForexEndpoints();
app.RegisterStockEndpoints();
app.RegisterCryptoEndpoints();
app.RegisterComodityEndpoints();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
