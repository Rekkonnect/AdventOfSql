namespace AdventOfSql.Infrastructure;

public sealed record ChallengeValidationResult(
    ChallengeOutput? Expected,
    ChallengeRunResult RunResult,
    ChallengeValidationResultType ValidationType)
{
    public DapperResult Output => RunResult.Result;
}
