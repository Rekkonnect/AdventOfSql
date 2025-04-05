namespace AdventOfSql.Infrastructure;

public sealed record ChallengeOutput(
    ChallengeIdentifier Identifier,
    QueryOutput? Output)
{
    public string? OutputString => Output?.ToString();
}
