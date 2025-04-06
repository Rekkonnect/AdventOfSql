namespace AdventOfSql.Infrastructure;

public sealed record ChallengeRunReport(
    ChallengeIdentifier Identifier)
{
    public DapperResult? Result { get; set; }
    public TimeDuration? SchemaTime { get; set; }
    public TimeDuration? InputTime { get; set; }
    public TimeDuration? SolveTime { get; set; }

    public bool HasFinished => Result is not null;
}
