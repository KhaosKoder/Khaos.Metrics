using System;
using Khaos.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace Khaos.Metrics.AspNet;

/// <summary>
/// Provides a reusable base for manually created monitoring controllers without registering routes by default.
/// </summary>
public abstract class MonitoringControllerBase : ControllerBase
{
	private readonly IMonitoringSnapshotProvider _snapshots;

	protected MonitoringControllerBase(IMonitoringSnapshotProvider snapshots)
	{
		_snapshots = snapshots ?? throw new ArgumentNullException(nameof(snapshots));
	}

	[NonAction]
	protected ActionResult<MonitoringSnapshot> GetSnapshotCore()
		=> Ok(_snapshots.GetSnapshot());
}
