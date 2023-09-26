using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Redemption.Balancer.Api.Application.Common.Contracts;
using Redemption.Balancer.Api.Application.Common.Extensions;
using Redemption.Balancer.Api.Application.Common.Factories;
using Redemption.Balancer.Api.Application.Common.Models.Configs;
using Redemption.Balancer.Api.Application.Tracing;
using Redemption.Balancer.Api.Application.Wrappers;
using Redemption.Balancer.Api.Constants;
using Redemption.Balancer.Api.Infrastructure.Clients;
using Redemption.Balancer.Api.Infrastructure.Common;
using Redemption.Balancer.Api.Infrastructure.Persistence;
using Redemption.Balancer.Api.Infrastructure.Services;
using Redemption.Balancer.Api.Infrastructure.Workers;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using StexchangeClient.Extensions.DependencyInjection;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

ConfigureLog();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<ResultFilter>();
    options.Filters.Add<RoleFilter>();
});

builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
        options.SwaggerDoc(
            Application.ApiVersion, new OpenApiInfo { Title = Application.ApiTitle, Version = Application.ApiVersion })
//uncomment for v4
//options.SwaggerDoc("v4", new() { Title = Application.ApiTitle, Version = "v4" });
);
builder.Services.AddApiVersioning(setup =>
{
    setup.DefaultApiVersion = new ApiVersion(3, 0);
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

RegisterDbContext();
RegisterServices();
RegisterClients();
BindConfiguration();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<BalancerDbContext>();

    await dataContext.Database.MigrateAsync();
}

//if (app.Environment.IsProduction() is false)
//{
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    foreach (var description in app.Services.GetRequiredService<IApiVersionDescriptionProvider>()
                 .ApiVersionDescriptions)
        options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
});
//}

app.UseRequestTracing();
app.MapHealthChecks("/health");
app.MapControllers();

await app.RunAsync();

void RegisterDbContext()
{
    var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

    var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

    var dataSource = dataSourceBuilder.Build();

    builder.Services.AddDbContext<BalancerDbContext>(c => c.UseNpgsql(dataSource!));
}

void RegisterServices()
{
    var serviceTypes = Assembly.GetExecutingAssembly()
        .GetTypes()
        .Where(type => type is { IsClass: true, IsAbstract: false } && type.Name.EndsWith("Service") && type.Name.StartsWith("Scheduling") is false);

    foreach (var serviceType in serviceTypes)
    {
        var interfaceType = serviceType.GetInterfaces().FirstOrDefault();

        if (interfaceType != null) builder.Services.AddScoped(interfaceType, serviceType);
    }

    builder.Services.AddScoped<IResponseFactory, ResponseFactory>();
    builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
    Assembly.GetEntryAssembly()?.GetTypesAssignableFrom<BaseBalancer>().ForEach(t =>
    {
        builder.Services.AddTransient(typeof(IBalancer), t);
    });

    builder.Services.AddHostedService<SchedulingBackgroundService>();

    var exchangeConfig = builder.Configuration.GetSection("ExchangeConfig").Get<ExchangeConfig>();
    builder.Services.AddStexchangeClient(exchangeConfig!.ClientUrl.TrimEnd('/'));
}

void BindConfiguration()
{
    builder.Services.Configure<BasicConfig>(settings =>
    {
        builder.Configuration.GetSection("BasicConfig").Bind(settings);
    });

    builder.Services.Configure<KenesConfig>(settings =>
    {
        builder.Configuration.GetSection("KenesConfig").Bind(settings);
    });

    builder.Services.Configure<StemeraldConfig>(settings =>
    {
        builder.Configuration.GetSection("StemeraldConfig").Bind(settings);
    });
}

void RegisterClients()
{
    builder.Services.AddHttpClient<IKenesClient, KenesClient>(option =>
    {
        var kenesConfig = builder.Configuration.GetSection("KenesConfig").Get<KenesConfig>();

        option.BaseAddress = new Uri(kenesConfig!.BaseAddress.TrimEnd('/'));
    });

    builder.Services.AddHttpClient<IBasicClient, BasicClient>(option =>
    {
        var basicConfig = builder.Configuration.GetSection("BasicConfig").Get<BasicConfig>();

        option.BaseAddress = new Uri(basicConfig.BaseAddress.TrimEnd('/'));
    });

    builder.Services.AddHttpClient<IStemeraldClient, StemeraldClient>(option =>
    {
        var stemeraldConfig = builder.Configuration.GetSection("StemeraldConfig").Get<StemeraldConfig>();

        option.BaseAddress = new Uri(stemeraldConfig.BaseAddress.TrimEnd('/'));
    });
}

void ConfigureLog()
{
    builder.Logging.ClearProviders();

    builder.Host.UseSerilog((ctx, cfg) =>
    {
        cfg.ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("service_name", Application.AppName);

        cfg.WriteTo.Async(sinkCfg => sinkCfg.Console());

        cfg.WriteTo.Elasticsearch(ConfigureElasticSink(builder.Configuration, builder.Environment.EnvironmentName));
    });
}

ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
{
    return new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"]!))
    {
        AutoRegisterTemplate = true,
        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name?.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}-" + "{0:yyyy-MM-dd}"
    };
}
