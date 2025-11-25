using System;

namespace Khaos.Metrics;

public interface ISystemClock
{
    DateTimeOffset Now { get; }

    long TimestampTicks { get; }
}
