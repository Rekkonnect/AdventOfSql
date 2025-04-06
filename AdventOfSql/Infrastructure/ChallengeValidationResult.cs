namespace AdventOfSql.Infrastructure;

public sealed record ChallengeValidationResult(
    ChallengeOutput? Expected,
    ChallengeRunReport RunReport,
    ChallengeValidationResultType ValidationType)
{
    public DapperResult Output => RunReport.Result!;
}
