namespace AdventOfSql.Infrastructure;

public sealed class ChallengeRunFlow
{
    public StepTrackParent DatabaseParent { get; }
    public StepTrack ConnectionTime { get; } = new("Connection");
    public StepTrack EnsureDatabaseExistsTime { get; } = new("Ensure Exists");
    public StepTrack DeleteSchemaTime { get; } = new("Delete Schema");

    public StepTrackParent SupplementaryParent { get; }
    public StepTrack LoadSupplementaryFileTime { get; } = new("Load File");
    public StepTrack ConstructSupplementaryTime { get; } = new("Construct");

    public StepTrackParent SchemaParent { get; }
    public StepTrack LoadSchemaFileTime { get; } = new("Load File");
    public StepTrack ConstructSchemaTime { get; } = new("Construct");

    public StepTrackParent InputParent { get; }
    public StepTrack LoadInputFileTime { get; } = new("Load File");
    public StepTrack InputTime { get; } = new("Run");

    public StepTrackParent SolveParent { get; }
    public StepTrack LoadSolveFileTime { get; } = new("Load File");
    public StepTrack SolveTime { get; } = new("Run");

    public IReadOnlyList<StepTrackParent> Parents
        => [
            DatabaseParent,
            SupplementaryParent,
            SchemaParent,
            InputParent,
            SolveParent,
        ];

    public ChallengeRunFlow()
    {
        DatabaseParent = new(
            "Database",
            [
                ConnectionTime,
                EnsureDatabaseExistsTime,
                DeleteSchemaTime,
            ]);

        SupplementaryParent = new(
            "Supplementary",
            [
                LoadSupplementaryFileTime,
                ConstructSupplementaryTime,
            ]);

        SchemaParent = new(
            "Schema",
            [
                LoadSchemaFileTime,
                ConstructSchemaTime,
            ]);

        InputParent = new(
            "Input",
            [
                LoadInputFileTime,
                InputTime,
            ]);

        SolveParent = new(
            "Solve",
            [
                LoadSolveFileTime,
                SolveTime,
            ]);
    }
}
