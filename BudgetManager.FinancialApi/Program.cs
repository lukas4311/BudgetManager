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
using BudgetManager.Core.SystemWrappers;
using BudgetManager.WebCore;
using Scalar.AspNetCore;
using BudgetManager.FinancialApi.Services;
using Microsoft.AspNetCore.Http.Features;
using BudgetManager.FinancialApi.Enums;
using BudgetManager.Client.FinancialApiClient;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureHostWithSerilogToElk();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

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

builder.Services.AddHttpClient(nameof(HttpClientKeys.FinApi), client =>
{
    FinApi finApi = builder.Configuration.GetSection(nameof(FinApi)).Get<FinApi>();
    client.BaseAddress = new Uri(finApi.Url);
});

builder.Services.Configure<JsnOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
    optionsBuilder.UseSqlServer(configuration.GetSection($"{nameof(DbSetting)}:ConnectionString").Value);
    containerBuilder.Register(_ => new DataContext(optionsBuilder.Options));
    containerBuilder.RegisterRepositories();
    containerBuilder.RegisterType<DateTimeWrap>().As<IDateTime>();
    containerBuilder.RegisterModelMapping();

    InfluxSetting influxSetting = configuration.GetSection("Influxdb").Get<InfluxSetting>();
    containerBuilder.RegisterServices();
    containerBuilder.RegisterInstance(new InfluxContext(influxSetting.Url, influxSetting.Token, influxSetting.OrganizationId)).As<IInfluxContext>();
    containerBuilder.RegisterType<CryptoData>();
    containerBuilder.RegisterType<ForexData>();
    containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
    containerBuilder.Register(ctx =>
    {
        IHttpClientFactory httpClientFactory = ctx.Resolve<IHttpClientFactory>();
        HttpClient client = httpClientFactory.CreateClient(nameof(HttpClientKeys.FinApi));
        return new FinancialClient(client);
    }).As<IFinancialClient>().InstancePerLifetimeScope();
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Scalar Example";
        opt.Theme = ScalarTheme.DeepSpace;
        opt.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.Http);
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.RegisterForexEndpoints();
app.RegisterStockEndpoints();
app.RegisterCryptoEndpoints();
app.RegisterComodityEndpoints();
app.Run();
