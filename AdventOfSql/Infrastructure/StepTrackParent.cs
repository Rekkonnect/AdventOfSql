using System.Collections.Immutable;

namespace AdventOfSql.Infrastructure;

public sealed record StepTrackParent(
    string Label,
    ImmutableArray<StepTrack> Tracks)
    : ILabelled
{
    public TimeSpan Elapsed()
    {
        var first = Tracks.First();
        var last = Tracks.Last();
        return last.FromOtherStartingStep(first);
    }
}
