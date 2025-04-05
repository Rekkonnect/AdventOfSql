namespace AdventOfSql.Infrastructure;

public sealed class UnsolvedChallengeException(string message)
    : Exception(message);
