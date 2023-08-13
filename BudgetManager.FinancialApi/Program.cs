using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using BudgetManager.FinancialApi.Models;
using BudgetManager.InfluxDbData;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

// Replace the built-in service provider with Autofac
builder.Services.AddAutofac(containerBuilder =>
{
    InfluxSetting influxSetting = configuration.GetSection("Influxdb").Get<InfluxSetting>();
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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
