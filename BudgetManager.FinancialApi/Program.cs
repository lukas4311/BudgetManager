using Autofac.Extensions.DependencyInjection;
using Autofac;
using BudgetManager.FinancialApi.Models;
using BudgetManager.InfluxDbData;
using BudgetManager.Services.Extensions;
using BudgetManager.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using BudgetManager.FinancialApi.Enums;
using System.Text.Json.Serialization;
using JsnOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

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

app.MapGet("/forex/exchangerate/{from}/{to}", async ([FromServices] IForexService forexService, [FromRoute] CurrencySymbol from, [FromRoute] CurrencySymbol to) =>
{
    if (from == to)
        return Results.Ok(1);

    var data = await forexService.GetExchangeRate(from.ToString(), to.ToString());
    return Results.Ok(data);
})
.WithName("GetForexPairPrice")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
