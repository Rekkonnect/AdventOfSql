﻿namespace AdventOfSql.Infrastructure;

public readonly record struct ChallengeIdentifier(
    ChallengeDayIdentifier DayIdentifier,
    TestCaseIdentifier TestCaseIdentifier)
{
    public string InputFileName
    {
        get
        {
            if (!TestCaseIdentifier.IsTestCase)
            {
                return DayIdentifier.Day.ToString();
            }

            return $"{DayIdentifier.Day}T{TestCaseIdentifier.TestCase}";
        }
    }

    public ChallengeIdentifier(
        int year,
        int day,
        TestCaseIdentifier testCase)
        : this(new(year, day), testCase)
    {
    }

    public ChallengeIdentifier(int year, int day)
        : this(year, day, TestCaseIdentifier.RealInput)
    {
    }

    public ChallengeIdentifier WithRealInputIdentifier()
    {
        return this with
        {
            TestCaseIdentifier = TestCaseIdentifier.RealInput,
        };
    }
}
