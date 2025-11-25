using System;
using System.Diagnostics;

namespace Khaos.Metrics;

public sealed class LocalSystemClock : ISystemClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public long TimestampTicks => Stopwatch.GetTimestamp();
}
