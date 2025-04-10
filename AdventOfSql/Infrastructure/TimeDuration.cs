using System.Diagnostics;

namespace AdventOfSql.Infrastructure;

public sealed class TimeDuration(long timestamp)
{
    private const long _uninitializedTimestamp = long.MaxValue;

    private long _timestamp = timestamp;
    private TimeSpan _duration = TimeSpan.MinValue;

    public TimeSpan Duration => _duration;

    // When the timestamp is uninitialized, the duration-related methods will
    // not yield valuable results, but no checks are performed intentionally
    public bool IsUninitialized => _timestamp is _uninitializedTimestamp;

    public bool IsFinished => _duration >= TimeSpan.Zero;

    public TimeDuration()
        : this(long.MaxValue) { }

    public TimeSpan FinishedOrCurrentDuration()
    {
        if (!IsFinished)
        {
            return CurrentDuration();
        }

        return _duration;
    }

    public void SetStartNow()
    {
        _timestamp = Stopwatch.GetTimestamp();
    }

    private TimeSpan CurrentDuration()
    {
        return Stopwatch.GetElapsedTime(_timestamp);
    }

    public void SetCurrentDuration()
    {
        _duration = CurrentDuration();
    }

    public static TimeDuration ForNow()
    {
        return new(Stopwatch.GetTimestamp());
    }

    public TimeSpan FromOtherStartingDuration(TimeDuration start)
    {
        if (IsFinished)
        {
            // Not too peak performance, but does the job with little friction for
            // the current model which is not all too clever
            var missingTime = Stopwatch.GetElapsedTime(start._timestamp, _timestamp);
            return _duration + missingTime;
        }

        return Stopwatch.GetElapsedTime(start._timestamp);
    }

    public DurationBlock TrackDuration()
    {
        SetStartNow();
        return new(this);
    }

    public readonly record struct DurationBlock(TimeDuration Duration)
        : IDisposable
    {
        public void Dispose()
        {
            Duration.SetCurrentDuration();
        }
    }
}
