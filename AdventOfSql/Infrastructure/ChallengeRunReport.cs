namespace AdventOfSql.Infrastructure;

public sealed record ChallengeRunReport(
    ChallengeIdentifier Identifier)
{
    public DapperResult? Result { get; set; }

    public TimeDuration ConnectionTime { get; } = new();
    public TimeDuration EnsureDatabaseExistsTime { get; } = new();

    public TimeDuration DeleteSchemaTime { get; } = new();

    public TimeDuration LoadSupplementaryFileTime { get; } = new();
    public TimeDuration ConstructSupplementaryTime { get; } = new();

    public TimeDuration LoadSchemaFileTime { get; } = new();
    public TimeDuration ConstructSchemaTime { get; } = new();

    public TimeDuration LoadInputFileTime { get; } = new();
    public TimeDuration InputTime { get; } = new();

    public TimeDuration LoadSolveFileTime { get; } = new();
    public TimeDuration SolveTime { get; } = new();

    public bool HasFinished => Result is not null;
}
