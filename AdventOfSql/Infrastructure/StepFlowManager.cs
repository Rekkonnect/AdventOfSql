namespace AdventOfSql.Infrastructure;

public sealed class StepFlowManager
{
    private StepTrack? _currentStep;

    public void BeginTracking(StepTrack? track)
    {
        _currentStep?.SetCurrentDuration();

        _currentStep = track;
        track?.SetStartNow();
    }

    public void Finish()
    {
        BeginTracking(null);
    }
}
