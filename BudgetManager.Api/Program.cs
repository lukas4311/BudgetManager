using Asp.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BudgetManager.Api.Enums;
using BudgetManager.Api.Middlewares;
using BudgetManager.Api.Models;
using BudgetManager.Api.Services;
using BudgetManager.Api.Services.SettingModels;
using BudgetManager.Client.FinancialApiClient;
using BudgetManager.Core.SystemWrappers;
using BudgetManager.Data;
using BudgetManager.Domain.MessagingContracts;
using BudgetManager.InfluxDbData;
using BudgetManager.Repository.Extensions;
using BudgetManager.Services.Contracts;
using BudgetManager.Services.Extensions;
using BudgetManager.WebCore;
using BudgetManager.WebCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Scalar.AspNetCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json.Serialization;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.JsonOptions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<AuthApiSetting>(builder.Configuration.GetSection("AuthApi"));
builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.Configure<MvcJsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddHttpClient();
builder.Services.AddAuthorization();

RabbitMqConfig rabbitSetting = builder.Configuration.GetSection("Rabbit").Get<RabbitMqConfig>();
rabbitSetting.EndpointsConfiguration = c => c.Publish<TickerRequest>(x => x.ExchangeType = ExchangeType.Direct);
builder.Services.AddMassTransitWithRabbitMq(rabbitSetting);

builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
    config.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"),
        new MediaTypeApiVersionReader("ver"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        System.Diagnostics.Activity activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

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
builder.Services.AddSwaggerGen(c =>
{
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

builder.Services.AddHttpClient(nameof(HttpClientKeys.FinApi), client =>
{
    FinApi finApi = builder.Configuration.GetSection(nameof(FinApi)).Get<FinApi>();
    client.BaseAddress = new Uri(finApi.Url);
});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.Register<IHttpContextAccessor>(a => new HttpContextAccessor());
    DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
    optionsBuilder.UseSqlServer(builder.Configuration.GetSection($"{nameof(DbSetting)}:ConnectionString").Value);
    containerBuilder.Register(_ => new DataContext(optionsBuilder.Options));
    containerBuilder.RegisterType<UserDataProviderService>().As<IUserDataProviderService>();
    containerBuilder.RegisterType<DateTimeWrap>().As<IDateTime>();
    containerBuilder.RegisterRepositories();
    containerBuilder.RegisterServices();

    InfluxSetting influxSetting = builder.Configuration.GetSection("Influxdb").Get<InfluxSetting>();
    containerBuilder.RegisterInstance(new InfluxContext(influxSetting.Url, influxSetting.Token, influxSetting.OrganizationId)).As<IInfluxContext>();
    containerBuilder.RegisterType<CryptoData>();
    containerBuilder.RegisterType<ForexData>();
    containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();
    containerBuilder.RegisterModelMapping();
    containerBuilder.Register(ctx =>
    {
        IHttpClientFactory httpClientFactory = ctx.Resolve<IHttpClientFactory>();
        HttpClient client = httpClientFactory.CreateClient(nameof(HttpClientKeys.FinApi));
        return new FinancialClient(client);
    }).As<IFinancialClient>().InstancePerLifetimeScope();
});

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "/openapi/{documentName}.json";
    });
    app.MapScalarApiReference(opt =>
    {
        opt.Title = "Scalar Example";
        opt.Theme = ScalarTheme.DeepSpace;
        opt.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.Http);
        opt.AddDocument("v1", "/openapi/v1.0.json");
    });
}
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();