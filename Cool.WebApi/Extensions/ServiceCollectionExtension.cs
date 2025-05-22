using Cool.Application.Interfaces.Repositories;
using Cool.Application.Interfaces.Services;
using Cool.Infrastructure.RabbitMq;
using Cool.Persistence.Context;
using Cool.Persistence.Resolver;
using Cool.Persistence.Services;
using Cool.Persistence.UoW;
using Jarvis.Application.MultiTenancy;
using Jarvis.Persistence;
using Jarvis.Persistence.MultiTenancy;
using Jarvis.WebApi.Monitoring;
using Jarvis.WebApi.Monitoring.Uptrace;
using Microsoft.EntityFrameworkCore;

namespace Cool.WebApi.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddHttpConnectionStringResolver(this IServiceCollection services)
    {
        services.AddScoped<HttpStorageConnectionStringResolver>();
        return services;
    }

    public static IServiceCollection AddMasterContext(this IServiceCollection services)
    {
        services.AddCoreDbContext<MasterDbContext, ConfigConnectionStringResolver>(
            (resolver, options) => options.UseNpgsql(resolver.GetConnectionString(nameof(MasterDbContext))));
        return services;
    }

    public static IServiceCollection AddAppContext(this IServiceCollection services)
    {
        services.AddCoreDbContext<AppDbContext, HttpStorageConnectionStringResolver>(
            (resolver, options) => options.UseNpgsql(resolver.GetConnectionString(nameof(AppDbContext))));
        return services;
    }

    public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        RabbitMQOption options = new();
        configuration.GetSection("RabbitMq").Bind(options);
        services.Configure<RabbitMQOption>(configuration.GetSection("RabbitMq"));
        services.Configure<RabbitMQOption>(config =>
        {
            config.Hosts = options.Hosts;
            config.Password = options.Password;
            config.UserName = options.UserName;
            config.VirtualHost = options.VirtualHost;
        });
        services.AddSingleton<IRabbitMQConnector, RabbitMQConnector>();
        services.AddSingleton<IDistributedEventProducer, BaseEventProducer>();
        return services;
    }

    public static IServiceCollection AddMultiTenancy(this IServiceCollection services)
    {
        services.AddScoped<ITenantIdResolver, HeaderTenantIdResolver>();
        services.AddScoped<ITenantIdResolver, QueryTenantIdResolver>();
        services.AddScoped<ITenantIdResolver, HostTenantIdResolver>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMasterUnitOfWork, MasterUnitOfWork>();
        services.AddScoped<IAppUnitOfWork, AppUnitOfWork>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFileService, FileService>();
        return services;
    }

    public static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        OTLPType.TraceExporters.Add(OTLPOption.ExporterType.OTLP, typeof(OTLPTraceExporter).AssemblyQualifiedName);
        OTLPType.MetricExporters.Add(OTLPOption.ExporterType.OTLP, typeof(OTLPMetricExporter).AssemblyQualifiedName);
        OTLPType.LoggingExporters.Add(OTLPOption.ExporterType.OTLP, typeof(OTLPLogExporter).AssemblyQualifiedName);

        return services;
    }
}
