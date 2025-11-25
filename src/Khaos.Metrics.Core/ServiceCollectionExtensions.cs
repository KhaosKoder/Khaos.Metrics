using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Khaos.Metrics;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKhaosMetrics(this IServiceCollection services, Action<MonitoringOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<MonitoringOptions>();
        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<ISystemClock>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<MonitoringOptions>>().Value;
            return options.TimeMode == MonitoringTimeMode.Local
                ? new LocalSystemClock()
                : new UtcSystemClock();
        });

        services.TryAddSingleton<MonitoringEngine>();
        services.TryAddSingleton<IMonitoringDataSource>(sp => sp.GetRequiredService<MonitoringEngine>());
        services.TryAddSingleton<IOperationMonitor>(sp =>
        {
            var engine = sp.GetRequiredService<MonitoringEngine>();
            var clock = sp.GetRequiredService<ISystemClock>();
            var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<MonitoringOptions>>();
            return new OperationMonitor(engine, clock, optionsMonitor);
        });
        services.TryAddSingleton<IMonitoringSnapshotProvider>(sp =>
        {
            var dataSource = sp.GetRequiredService<IMonitoringDataSource>();
            var clock = sp.GetRequiredService<ISystemClock>();
            return new MonitoringSnapshotProvider(dataSource, clock);
        });

        return services;
    }
}
