namespace AdventOfSql.Infrastructure;

public sealed class TimeDurationFlowManager
{
    private TimeDuration? _currentDuration;

    public void BeginTracking(TimeDuration? duration)
    {
        _currentDuration?.SetCurrentDuration();

        _currentDuration = duration;
        duration?.SetStartNow();
    }

    public void Finish()
    {
        BeginTracking(null);
    }
}
