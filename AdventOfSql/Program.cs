using AdventOfSql.Infrastructure;
using Garyon.Objects;

IReadOnlyList<ChallengeIdentifier> identifiers = [
    new(2024, 01),
    new(2024, 02),
    new(2024, 03),
    new(2024, 05),
    new(2024, 06),
    new(2024, 07),
];
await Singleton<ConsoleChallengeRunner>.Instance.RunMany(identifiers);
