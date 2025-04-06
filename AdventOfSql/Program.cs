using AdventOfSql.Infrastructure;
using Garyon.Objects;

IReadOnlyList<ChallengeIdentifier> identifiers = [
    new ChallengeIdentifier(2024, 01, TestCaseIdentifier.RealInput),
    new ChallengeIdentifier(2024, 02, TestCaseIdentifier.RealInput),
    new ChallengeIdentifier(2024, 03, TestCaseIdentifier.RealInput),
    new ChallengeIdentifier(2024, 05, TestCaseIdentifier.RealInput),
    new ChallengeIdentifier(2024, 06, TestCaseIdentifier.RealInput),
];
await Singleton<ConsoleChallengeRunner>.Instance.RunMany(identifiers);
