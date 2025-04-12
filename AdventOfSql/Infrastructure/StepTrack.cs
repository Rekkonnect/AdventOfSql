using System.Diagnostics;

namespace AdventOfSql.Infrastructure;

public sealed class StepTrack(string label)
    : ILabelled
{
    private const long _uninitializedTimestamp = long.MaxValue;

    private long _start = _uninitializedTimestamp;
    private TimeSpan _duration = TimeSpan.MinValue;

    public TimeSpan Duration => _duration;

    // When the timestamp is uninitialized, the duration-related methods will
    // not yield valuable results, but no checks are performed intentionally
    public bool IsUninitialized => _start is _uninitializedTimestamp;

    public bool IsFinished => _duration >= TimeSpan.Zero;

    public string Label { get; } = label;

    public StepTrack()
        : this(string.Empty) { }

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
        _start = Stopwatch.GetTimestamp();
    }

    private TimeSpan CurrentDuration()
    {
        return Stopwatch.GetElapsedTime(_start);
    }

    public void SetCurrentDuration()
    {
        _duration = CurrentDuration();
    }

    public TimeSpan FromOtherStartingStep(StepTrack start)
    {
        if (IsFinished)
        {
            // Not too peak performance, but does the job with little friction for
            // the current model which is not all too clever
            var missingTime = Stopwatch.GetElapsedTime(start._start, _start);
            return _duration + missingTime;
        }

        return Stopwatch.GetElapsedTime(start._start);
    }

    public TrackBlock BeginBlock()
    {
        SetStartNow();
        return new(this);
    }

    public readonly record struct TrackBlock(StepTrack Track)
        : IDisposable
    {
        public void Dispose()
        {
            Track.SetCurrentDuration();
        }
    }
}
