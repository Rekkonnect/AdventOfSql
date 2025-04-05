namespace AdventOfSql.Infrastructure;

public sealed record ChallengeRunResult(
    ChallengeIdentifier Identifier,
    DapperResult Result,
    TimeSpan SchemaTime,
    TimeSpan InputTime,
    TimeSpan SolveTime);
