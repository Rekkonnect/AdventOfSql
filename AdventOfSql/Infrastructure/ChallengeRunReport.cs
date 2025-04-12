namespace AdventOfSql.Infrastructure;

public sealed record ChallengeRunReport(
    ChallengeIdentifier Identifier)
{
    public DapperResult? Result { get; set; }
    public Exception? Exception { get; set; }

    public ChallengeRunFlow RunFlow { get; } = new();

    public bool HasFinished
        => Result is not null
        || Exception is not null
        ;
}
