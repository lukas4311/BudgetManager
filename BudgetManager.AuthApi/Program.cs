using Asp.Versioning;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BudgetManager.AuthApi.Models;
using BudgetManager.Data;
using BudgetManager.Repository.Extensions;
using BudgetManager.Services.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<JwtSettingOption>(builder.Configuration.GetSection(nameof(JwtSettingOption)));
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
}).AddMvc() // This is needed for controllers
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://localhost:44386", "https://localhost:5001")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder.Register<IHttpContextAccessor>(a => new HttpContextAccessor());
    DbContextOptionsBuilder<DataContext> optionsBuilder = new DbContextOptionsBuilder<DataContext>();
    optionsBuilder.UseSqlServer(builder.Configuration.GetSection($"{nameof(DbSetting)}:ConnectionString").Value);
    containerBuilder.Register<DataContext>(_ => new DataContext(optionsBuilder.Options));
    containerBuilder.RegisterType<ConfigureSwaggerOptions>().As<IConfigureOptions<SwaggerGenOptions>>();
    containerBuilder.RegisterRepositories();
    containerBuilder.RegisterServices();
    containerBuilder.RegisterModelMapping();
});

var app = builder.Build();

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
        opt.AddDocument("v2", "/openapi/v2.0.json");
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();
