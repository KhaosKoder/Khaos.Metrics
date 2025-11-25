# Khaos.Metrics

Khaos.Metrics is a .NET 8 library that provides bounded, in-memory metrics for logical operations. It focuses on throughput, latency, concurrency, and process CPU usage with minimal overhead and full control over event dispatch.

The repository contains:

- `Khaos.Metrics.Core`: monitoring engine, DI helpers, CPU sampler, and snapshot provider.
- `Khaos.Metrics.AspNet`: opt-in ASP.NET Core endpoints and controller helpers for exposing snapshots.
- Comprehensive xUnit tests and BenchmarkDotNet harnesses.

## Why Use It?

Reach for Khaos.Metrics when you need lightweight, bounded observability without a heavyweight telemetry stack:

- Track hot paths inside services with predictable memory usage (ring buffers).
- Export operation snapshots for dashboards or management APIs.
- React to slow operations through event sinks, with inline or background dispatch and drop policies.
- Capture process-level CPU alongside per-operation metrics.

## Quick Start

### 1. Register services

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddKhaosMetrics(options =>
{
	options.TimeMode = MonitoringTimeMode.Utc;
	options.SamplingRate = 1;
});
```

### 2. Instrument code

```csharp
public class CheckoutService
{
	private readonly IOperationMonitor _monitor;

	public CheckoutService(IOperationMonitor monitor) => _monitor = monitor;

	public async Task PlaceOrderAsync(Order order)
	{
		using var scope = _monitor.Begin("checkout", new OperationTags
		{
			{ "region", order.Region },
			{ "channel", order.Channel }
		});

		try
		{
			await _processor.RunAsync(order);
		}
		catch
		{
			scope.MarkFailed();
			throw;
		}
	}
}
```

### 3. Expose snapshot endpoint (optional)

```csharp
var app = builder.Build();
app.MapMonitoringEndpoints("/monitoring");
```

Now `GET /monitoring/snapshot` returns the current `MonitoringSnapshot`.

## Documentation

- [Architecture Overview](docs/architecture-overview.md)
- [Developer Integration Guide](docs/developer-integration-guide.md)
- [Configuration Reference](docs/configuration-reference.md)
- [Testing & Benchmarks](docs/testing-and-benchmarks.md)
- [Operations Playbook](docs/operations-playbook.md)
- [Versioning Guide](docs/versioning-guide.md)

## Building & Testing

```powershell
dotnet restore
dotnet build
dotnet test
```

See [docs/build-commands.md](docs/build-commands.md) for additional commands (coverage, cleaning, packaging, etc.).

## Versioning

The solution follows SemVer 2.0.0 with Git tags (`Khaos.Metrics/vX.Y.Z`) via MinVer. Refer to the [Versioning Guide](docs/versioning-guide.md) for tagging and release steps.
