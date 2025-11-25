using System;
using Khaos.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Khaos.Metrics.AspNet;

public static class MonitoringEndpointRouteBuilderExtensions
{
	public static IEndpointRouteBuilder MapMonitoringEndpoints(
		this IEndpointRouteBuilder endpoints,
		string pattern = "/monitoring")
	{
		ArgumentNullException.ThrowIfNull(endpoints);
		ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

		var group = endpoints.MapGroup(pattern);
		group.MapGet("/snapshot", (IMonitoringSnapshotProvider provider) => provider.GetSnapshot());
		return endpoints;
	}
}
