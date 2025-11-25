using System;
using System.Diagnostics;

namespace Khaos.Metrics;

public sealed class UtcSystemClock : ISystemClock
{
    public DateTimeOffset Now => DateTimeOffset.UtcNow;

    public long TimestampTicks => Stopwatch.GetTimestamp();
}
