using System.Diagnostics;

namespace AdventOfSql.Infrastructure;

public sealed class TimeDuration(long timestamp)
{
    private TimeSpan _duration = TimeSpan.MinValue;

    public readonly long Timestamp = timestamp;
    public TimeSpan Duration => _duration;

    public TimeSpan FinishedOrCurrentDuration()
    {
        if (_duration < TimeSpan.Zero)
        {
            return CurrentDuration();
        }

        return _duration;
    }

    private TimeSpan CurrentDuration()
    {
        return Stopwatch.GetElapsedTime(Timestamp);
    }

    public void SetCurrentDuration()
    {
        _duration = CurrentDuration();
    }

    public static TimeDuration ForNow()
    {
        return new(Stopwatch.GetTimestamp());
    }
}
