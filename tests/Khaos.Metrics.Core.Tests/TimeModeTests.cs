using System;
using Khaos.Metrics.Core.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Khaos.Metrics.Core.Tests;

public class TimeModeTests
{
	[Fact]
	public void ServiceCollectionUsesLocalClockWhenConfigured()
	{
		var services = new ServiceCollection();
		services.AddKhaosMetrics(o => o.TimeMode = MonitoringTimeMode.Local);

		var provider = services.BuildServiceProvider();
		var clock = provider.GetRequiredService<ISystemClock>();

		Assert.IsType<LocalSystemClock>(clock);
	}

	[Fact]
	public void SnapshotUsesClockTimestamp()
	{
		var options = new MonitoringOptions
		{
			EventDispatchMode = EventDispatchMode.Inline
		};

		using var engine = new MonitoringEngine(new TestOptionsMonitor(options), Array.Empty<IOperationEventSink>());
		var clock = new TestClock(DateTimeOffset.Parse("2025-01-01T00:00:00Z"));
		var provider = new MonitoringSnapshotProvider(engine, clock);

		var snapshot = provider.GetSnapshot();

		Assert.Equal(clock.Now, snapshot.GeneratedAt);
	}
}
