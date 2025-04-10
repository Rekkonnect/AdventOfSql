using AdventOfSql.Infrastructure;
using Garyon.Objects;

await Singleton<ConsoleChallengeRunner>.Instance
    .RunMany([
        new(2024, 16, 1),
        new(2024, 16),
    ]);
