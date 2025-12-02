using System;
using System.Diagnostics;
using Khaos.Time;

namespace Khaos.Metrics.Core.Tests.Infrastructure;

internal sealed class TestClock : ISystemClock
{
	private DateTimeOffset _now;
	private long _timestampTicks;

	public TestClock(DateTimeOffset? start = null)
	{
		_now = start ?? DateTimeOffset.Now;
	}

	public DateTimeOffset Now => _now;

	public DateTimeOffset UtcNow => _now.ToUniversalTime();

	public long TimestampTicks => _timestampTicks;

	public void Advance(TimeSpan amount)
	{
		_now += amount;
		_timestampTicks += ToStopwatchTicks(amount);
	}

	public static long ToStopwatchTicks(TimeSpan duration)
		=> (long)(duration.TotalSeconds * Stopwatch.Frequency);
}
