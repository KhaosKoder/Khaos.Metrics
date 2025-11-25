using System.Collections.Generic;

namespace Khaos.Metrics;

public interface IMonitoringDataSource
{
    MonitoringOptions CurrentOptions { get; }

    IEnumerable<OperationMetricsSnapshotData> CaptureSnapshots();
}
