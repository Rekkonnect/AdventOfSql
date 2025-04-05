namespace AdventOfSql.Infrastructure;

public readonly record struct ChallengeDayIdentifier(
    int Year,
    int Day)
{
    public ChallengeIdentifier WithTestCase(TestCaseIdentifier testCase)
    {
        return new(this, testCase);
    }
}
